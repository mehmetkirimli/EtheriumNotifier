using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Application.Dto
{
    public class CreateNotificationChannelRequestDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int UserId { get; set; }
        public string WatchedAddress { get; set; } = null!;
        public ChannelType ChannelType { get; set; }
        public string Target { get; set; } = null!;
    }
}
