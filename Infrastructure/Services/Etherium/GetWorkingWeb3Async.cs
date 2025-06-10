using Application.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Web3;

namespace Infrastructure.Services.Etherium
{
    public interface IWeb3Factory
    {
        Task<Web3> GetWorkingWeb3Async();
    }

    public class Web3Factory : IWeb3Factory
    {
        private readonly ILogger<Web3Factory> _logger;
        private readonly IOptions<EthereumOptions> _options;

        public Web3Factory(ILogger<Web3Factory> logger, IOptions<EthereumOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        public async Task<Web3> GetWorkingWeb3Async()
        {
            foreach (var url in _options.Value.RpcUrls)
            {
                var web3 = new Web3(url);
                try
                {
                    var blockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
                    return web3; // başarılıysa o Web3'ü kullan
                }
                catch(Exception ex)
                {
                    _logger.LogWarning(ex, $"RPC URL {url} başarısız oldu, diğerine geçiliyor...");
                }
            }

            throw new Exception("Tüm RPC URL'lerine bağlanılamadı!");
        }

     
    }

}
