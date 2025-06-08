using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.ServicesImpl;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.Blocks;
using Nethereum.Web3;

namespace Infrastructure.Services.Etherium
{
    public class EthereumService : IEthereumService
    {
        private readonly Web3 _web3;

        public EthereumService(string rpcUrl)
        {
            _web3 = new Web3(rpcUrl);
        }

        public async Task<List<ExternalTransactionDto>> GetRecentTransactionAsync(int blockCount = 100)
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
    }
}


/*/ Note 

Nethereum’un Ethereum ile RPC protokolünde çalışması için  
blok numaralarını hexadecimal formatta göndermesi gerekir.  

HexBigInteger, bu sayıyı uygun formata çevirir (örneğin: "0x12a4ef" gibi)  

*/
