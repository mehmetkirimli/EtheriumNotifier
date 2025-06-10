using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Dto
{
    public class NotificationLogDto
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public ChannelType ChannelType { get; set; }
        public string Target { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string TransactionHash { get; set; }
    }

}
