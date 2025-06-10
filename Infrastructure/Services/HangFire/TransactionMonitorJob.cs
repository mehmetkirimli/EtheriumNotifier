using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.ServicesImpl;

namespace Infrastructure.Services.HangFire
{
    public class TransactionMonitorJob
    {
        private readonly IEthereumService _ethereumService;
        public TransactionMonitorJob(IEthereumService ethereumService)
        {
            _ethereumService = ethereumService;
        }


        /// <summary>
        /// Ethereum zincirindeki son işlemleri kontrol eder ve veritabanına kaydeder.
        /// </summary>
        public async Task ExecuteAsync()
        {
            try
            {
                var newTransactions = await _ethereumService.SaveTransactionsAsync();
                if (newTransactions.Any()) {
                    await _ethereumService.NotifyAllAsync(newTransactions);
                }
            }
            catch (Exception ex)
            {
                // Hata loglama işlemi yapılabilir
                Console.WriteLine($"Error in TransactionMonitorJobs: {ex.Message}");
            }
        }
    }
}
