using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ExternalTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public string TransactionHash { get; set; } = null!; // her işlem hash ile unique olur
        public long BlockNumber { get; set; }
        public string BlockHash { get; set; } = null!;
        public int TransactionIndex { get; set; }
        public bool TransactionStatus { get; set; }
        public DateTime ProcessingTime { get; set; }
        public DateTime DbCreatedTime { get; set; } = DateTime.UtcNow;
    }

}
