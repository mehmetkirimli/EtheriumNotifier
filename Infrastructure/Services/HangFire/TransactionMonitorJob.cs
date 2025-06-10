using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ServicesImpl;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.HangFire
{
    public class TransactionMonitorJob
    {
        private readonly IEthereumService _ethereumService;
        private readonly ILogger<TransactionMonitorJob> _logger;
        public TransactionMonitorJob(IEthereumService ethereumService, ILogger<TransactionMonitorJob> logger)
        {
            _ethereumService = ethereumService;
            _logger = logger;
        }


        /// <summary>
        /// Ethereum zincirindeki son işlemleri kontrol eder ve veritabanına kaydeder.
        /// </summary>
        public async Task ExecuteAsync()
        {
            try
            {
                var newTransactions = await _ethereumService.SaveTransactionsAsync();
                if (newTransactions.Any()) { await _ethereumService.NotifyAllAsync(newTransactions); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TransactionMonitorJobs warning => {ex.Message}");
                _logger.LogWarning($"TransactionMonitorJob execution temporary error occurred in the working state : {ex.Message} ");
            }
        }
    }
}
