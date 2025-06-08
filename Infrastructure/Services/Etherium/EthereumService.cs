using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Options;
using Application.ServicesImpl;
using Domain.Entities;
using Infrastructure.Services.Redis;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<ExternalTransactionDto>> GetRecentTransactionAsync(int blockCount = 5)
        {
            IEthBlockNumber GetBlockNumberRequest = _web3.Eth.Blocks.GetBlockNumber; // IEthBlockNumber HexBigInteger'dan miras alınmış bir interface olduğu için, bu şekilde kullanabiliriz.
            var latestBlockNumber = await GetBlockNumberRequest.SendRequestAsync();
            var transactions = new List<ExternalTransactionDto>();

            for(var i =0; i< blockCount; i++) 
            {
                var blockNumber = new HexBigInteger(latestBlockNumber.Value - i);
                var blokc = await _web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockNumber);

                foreach(var tx in blokc.Transactions)
                {
                    if(tx.Value.Value > 0 && !string.IsNullOrEmpty(tx.To) )
                    {

                        transactions.Add(new ExternalTransactionDto
                        {
                            From = tx.From,
                            To = tx.To,
                            Hash = tx.TransactionHash,
                            Value = Web3.Convert.FromWei(tx.Value.Value),
                            BlockNumber = (ulong)blockNumber.Value
                        });
                    }
                }
            }
            return transactions;
        }

        public async Task FetchAndSaveRecentTransactionsAsync(int blockCount = 5)
        {
            var transactions = await GetRecentTransactionAsync(blockCount);

            foreach (var tx in transactions)
            {
                //var exists = await _externalTransactionRepository.GetFilteredAsync(e => e.Hash == tx.Hash); DB'ye gidip sormak demek zaten performans kaybettirir , bu sebeple ilk önlem redis ile 2.önlem hash değerinin unıque olması ile alınıyor

                // Redis üzerinden idempotency kontrolü
                bool alreadyExists = await _redisService.HasKeyAsync(tx.Hash);

                if (alreadyExists)
                {
                    _logger.LogInformation($"Transaction with hash {tx.Hash} already exists in Redis.");
                    continue;
                }

                // Eğer Redis'te yoksa, veritabanına eklenilebilir
                await _externalTransactionRepository.AddAsync(new ExternalTransaction
                {
                    From = tx.From,
                    To = tx.To,
                    Hash = tx.Hash,
                    Value = tx.Value,
                    BlockNumber = (long)tx.BlockNumber,
                    CreatedAt = DateTime.UtcNow
                });

                // Redis'e işlenmiş olarak yaz
                await _redisService.SaveResponseAsync(tx.Hash, true);
            }

            _logger.LogInformation("Fetch and save işlemi tamamlandı.");
        }
    }
}


/*/ Note 

Nethereum’un Ethereum ile RPC protokolünde çalışması için  
blok numaralarını hexadecimal formatta göndermesi gerekir.  

HexBigInteger, bu sayıyı uygun formata çevirir (örneğin: "0x12a4ef" gibi)  

*/
