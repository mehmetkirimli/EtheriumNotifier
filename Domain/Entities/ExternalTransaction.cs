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
        public string Hash { get; set; }
        public decimal Value { get; set; }
        public long BlockNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
