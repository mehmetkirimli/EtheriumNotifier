using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;

namespace Infrastructure.Services.HangFire
{
    public static class HangfireJobRegistration
    {
        public static void RegisterJobs() 
        {
            RecurringJob.AddOrUpdate<TransactionMonitorJob>("ethereum-transaction-monitor",
                job => job.ExecuteAsync(),
                Cron.Minutely); // Her dakika çalışacak şekilde ayarlandı
        }
    }
}
