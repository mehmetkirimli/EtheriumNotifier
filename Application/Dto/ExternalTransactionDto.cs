using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class ExternalTransactionDto
    {
        public string From { get; set; } = default!;
        public string To { get; set; } = default!;
        public string Hash { get; set; } = default!;
        public decimal Value { get; set; }
        public ulong BlockNumber { get; set; }
    }
}
