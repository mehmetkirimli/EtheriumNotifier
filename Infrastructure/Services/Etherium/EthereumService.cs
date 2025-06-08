using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.ServicesImpl;
using Nethereum.Web3;

namespace Infrastructure.Services.Etherium
{
    public class EthereumService : IEhtereumService
    {
        private readonly Web3 _web3;

        public EthereumService(string rpcUrl)
        {
            _web3 = new Web3(rpcUrl);
        }

        public Task<List<ExternalTransactionDto>> GetRecentTransactionAsync(int blockCount = 100)
        {
            return null;
        }
    }
}
