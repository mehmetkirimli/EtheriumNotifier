using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Dto
{
    public class NotificationLogFilterRequestDto
    {
        public int? UserId { get; set; }                        
        public List<ChannelType>? Channels { get; set; }       
        public DateTime? FromDate { get; set; }                 
        public DateTime? ToDate { get; set; }                  
    }

}
