using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class TransactionFilterRequestDto
    {
        public string? Address { get; set; }
        public decimal? MinAmount { get; set; }
        public int? BlockNumber { get; set; }
        public string? TransactionHash { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        // Opsiyonel: Pagination
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

}
