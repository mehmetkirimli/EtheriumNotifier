using Application.Dto;
using Application.Options;
using Application.ServicesImpl;
using Domain.Entities;
using Infrastructure.Services.Redis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.Blocks;
using Nethereum.Web3;
using Persistence.Repositories;

namespace Infrastructure.Services.Etherium
{
    public class EthereumService : IEthereumService
    {
        private readonly Web3 _web3;
        private readonly ILogger<EthereumService> _logger;
        private readonly IRepository<ExternalTransaction> _externalTransactionRepository;
        private readonly IRedisService _redisService;
        public EthereumService(IOptions<EthereumOptions> options,ILogger<EthereumService> logger,IRepository<ExternalTransaction> externalTransactionRepository , IRedisService redis)
        {
            _logger = logger;
            _externalTransactionRepository = externalTransactionRepository;
            _web3 = new Web3(options.Value.RpcUrl);
            _redisService = redis;
        }

        public async Task<List<ExternalTransactionDto>> FetchTransactionAsync(int blockCount = 10)
        {
            IEthBlockNumber GetBlockNumberRequest = _web3.Eth.Blocks.GetBlockNumber; // IEthBlockNumber HexBigInteger'dan miras alınmış bir interface olduğu için, bu şekilde kullanabiliriz.
            var latestBlockNumber = await GetBlockNumberRequest.SendRequestAsync();
            var transactions = new List<ExternalTransactionDto>();

            for(var i =0; i< blockCount; i++) 
            {
                var blockNumber = new HexBigInteger(latestBlockNumber.Value - i);
                var block = await _web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockNumber);

                foreach(var tx in block.Transactions)
                {
                    if(tx.Value.Value > 0 && !string.IsNullOrEmpty(tx.To) )
                    {
                        var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(tx.TransactionHash);
                        bool txStatus = receipt?.Status.Value == 1;                 // 0x1 ise başarılı, 0x0 ise başarısız

                        var timestamp = (long)block.Timestamp.Value; 
                        var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;

                        transactions.Add(new ExternalTransactionDto
                        {
                            From = tx.From,                                         //senderAddress
                            To = tx.To,                                             //receiverAddress
                            Amount = Web3.Convert.FromWei(tx.Value.Value),          //amount
                            TransactionHash = tx.TransactionHash,                   //transactionHash
                            BlockNumber = (long)blockNumber.Value,                  //blockNumber
                            BlockHash = block.BlockHash,                            //blockHash
                            TransactionIndex = (int)tx.TransactionIndex.Value,      //transactionIndex
                            TransactionStatus = txStatus,                           //tranasactionStatus 
                            ProcessingTime = dateTime                               //processingTime
                        });
                    }
                }
            }
            return transactions;
        }

        public async Task SaveTransactionsAsync(int blockCount =10)
        {
            var transactions = await FetchTransactionAsync(blockCount);

            foreach (var tx in transactions)
            {

                // Redis üzerinden idempotency kontrolü
                string redisKey = $"Tx-Hash:{tx.TransactionHash}";
                bool alreadyExists = await _redisService.HasKeyAsync(redisKey);

                if (alreadyExists)
                {
                    _logger.LogInformation($"Transaction with hash : {redisKey} already exists in Redis.");
                    continue;
                }

                // Eğer Redis'te yoksa, veritabanına eklenilebilir
                await _externalTransactionRepository.AddAsync(new ExternalTransaction
                {
                    From = tx.From,                                         //senderAddress
                    To = tx.To,                                             //receiverAddress
                    Amount = tx.Amount,                                     //amount
                    TransactionHash = tx.TransactionHash,                   //transactionHash
                    BlockNumber = tx.BlockNumber,                           //blockNumber
                    BlockHash = tx.BlockHash,                               //blockHash
                    TransactionIndex = tx.TransactionIndex,                 //transactionIndex
                    TransactionStatus = tx.TransactionStatus,               //tranasactionStatus 

                    ProcessingTime = tx.ProcessingTime,                     //processingTime
                    DbCreatedTime = DateTime.UtcNow
                });

                // Redis'e işlenmiş olarak yaz
                await _redisService.SaveResponseAsync(redisKey, true ,TimeSpan.FromMinutes(20));
                //await _redisService.AddHashToMinuteSetAsync(tx.TransactionHash, tx.ProcessingTime); Timeout hatası riski oluşturmasın diye yoruma aldım.
            }

            _logger.LogInformation("Get Transaction and Save process is succesfully .");
        }

        public Task<List<ExternalTransactionDto>> GetAllSavedTransactionsAsync()
        {
            // InMemory veritabanından tüm ExternalTransaction kayıtlarını getirir
            return _externalTransactionRepository.GetAllAsync()
                .ContinueWith(task => task.Result.Select(tx => new ExternalTransactionDto
                {
                    From = tx.From,
                    To = tx.To,
                    Amount = tx.Amount,
                    TransactionHash = tx.TransactionHash,
                    BlockNumber = tx.BlockNumber
                }).ToList());
        }

        public async Task<List<ExternalTransactionDto>> GetFilteredTransactionsAsync(TransactionFilterRequestDto dto)
        {
            // --- Validasyon ---
            dto.Address = dto.Address?.Trim();

            if (!string.IsNullOrEmpty(dto.Address) && (dto.Address.Length != 42 || !dto.Address.StartsWith("0x")))
                throw new ArgumentException("Adres formatı geçersiz!");

            if (!string.IsNullOrEmpty(dto.TransactionHash) && (dto.TransactionHash.Length != 66 || !dto.TransactionHash.StartsWith("0x")))
                throw new ArgumentException("Transaction-Hash formatı geçersiz!");

            if (dto.MinDate.HasValue && dto.MinDate < DateTime.UtcNow.AddMonths(-2))
                throw new ArgumentException("Min sorgulama tarihi 2 aydan eski olamaz!");

            if (dto.MinDate.HasValue && dto.MaxDate.HasValue && (dto.MaxDate - dto.MinDate).Value.TotalDays > 15)
                throw new ArgumentException("Tarih aralığı maksimum 15 gün olmalı!");

            // --- Filtreleme ---
            var all = await _externalTransactionRepository.GetAllAsync();
            var query = all.AsQueryable();

            if (!string.IsNullOrEmpty(dto.Address))
                query = query.Where(x => x.From == dto.Address || x.To == dto.Address);

            if (dto.MinAmount.HasValue)
                query = query.Where(x => x.Amount >= dto.MinAmount.Value);

            if (dto.BlockNumber.HasValue)
                query = query.Where(x => x.BlockNumber == dto.BlockNumber.Value);

            if (!string.IsNullOrEmpty(dto.TransactionHash))
                query = query.Where(x => x.TransactionHash == dto.TransactionHash);

            if (dto.MinDate.HasValue)
                query = query.Where(x => x.ProcessingTime >= dto.MinDate.Value);

            if (dto.MaxDate.HasValue)
                query = query.Where(x => x.ProcessingTime <= dto.MaxDate.Value);

            // --- Pagination ---
            if (dto.PageNumber.HasValue && dto.PageSize.HasValue)
                query = query.Skip((dto.PageNumber.Value - 1) * dto.PageSize.Value).Take(dto.PageSize.Value);


            // Dönüşüm: Entity'den DTO'ya geçişş
            var result = query.Select(tx => new ExternalTransactionDto
            {
                From = tx.From,
                To = tx.To,
                Amount = tx.Amount,
                TransactionHash = tx.TransactionHash,
                BlockNumber = tx.BlockNumber,
                BlockHash = tx.BlockHash,
                TransactionIndex = tx.TransactionIndex,
                TransactionStatus = tx.TransactionStatus,
                ProcessingTime = tx.ProcessingTime
            }).ToList();

            return result;
        }

        public async Task<ExternalTransactionDto> GetTransactionByHashAsync(string hash)
        {
            if (string.IsNullOrEmpty(hash) || hash.Length != 66 || !hash.StartsWith("0x"))
                throw new ArgumentException("Transaction hash formatı geçersiz!");

            var all = await _externalTransactionRepository.GetAllAsync();
            var tx = all.FirstOrDefault(x => x.TransactionHash == hash);

            if (tx == null)
                throw new KeyNotFoundException("Transaction bulunamadı!");

            // DTO map
            return new ExternalTransactionDto
            {
                From = tx.From,
                To = tx.To,
                Amount = tx.Amount,
                TransactionHash = tx.TransactionHash,
                BlockNumber = tx.BlockNumber,
                BlockHash = tx.BlockHash,
                TransactionIndex = tx.TransactionIndex,
                TransactionStatus = tx.TransactionStatus,
                ProcessingTime = tx.ProcessingTime
            };
        }
    }
}

/*/ Note 

Nethereum’un Ethereum ile RPC protokolünde çalışması için  
blok numaralarını hexadecimal formatta göndermesi gerekir.  

HexBigInteger, bu sayıyı uygun formata çevirir (örneğin: "0x12a4ef" gibi)  

*/
