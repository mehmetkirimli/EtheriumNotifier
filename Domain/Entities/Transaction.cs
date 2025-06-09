using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Hash { get; set; } = null!; // her işlem hash ile unique olur
        public string From { get; set; } = null!;
        public string To { get; set; } = null!;
        public decimal Amount { get; set; }
        public string BlockHash { get; set; } = null!;
        public long BlockNumber { get; set; }
        public int TransactionIndex { get; set; }
        public bool TransactionStatus { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
