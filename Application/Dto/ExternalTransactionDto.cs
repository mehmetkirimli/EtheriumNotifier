using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class ExternalTransactionDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public string TransactionHash { get; set; } = null!; 
        public long BlockNumber { get; set; }
        public string BlockHash { get; set; } = null!;
        public int TransactionIndex { get; set; }
        public bool TransactionStatus { get; set; } // 0x0 veya 0x1
        public DateTime ProcessingTime { get; set; }
    }
}
