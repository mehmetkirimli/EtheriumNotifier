using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.ServicesImpl;
using Domain.Entities;
using Domain.Enums;
using Persistence.Repositories;

namespace Persistence.Seed
{
    public class ChannelSeeder
    {
        private readonly INotificationChannelService _service;
        public ChannelSeeder(INotificationChannelService service)
        {
            _service = service;
        }
        public async Task SeedAsync()
        {
            var channels = new List<CreateNotificationChannelRequestDto>
        {
            new() { UserId = 1001, WatchedAddress = "0xdE2fACa4BBC0aca08fF04D387c39B6f6325bf82A", ChannelType = ChannelType.Email, Target = "mail1@example.com" },
            new() { UserId = 1002, WatchedAddress = "0x28C6c06298d514Db089934071355E5743bf21d60", ChannelType = ChannelType.Slack, Target = "slack-xyz"},
            new() { UserId = 1003, WatchedAddress = "0x6cc9397c3b38739dacbfaa68ead5f5d77ba5f455", ChannelType = ChannelType.SMS, Target = "05544405806" },
            new() { UserId = 1004, WatchedAddress = "0x233e416b0897e8f4796d89a84b5ade4365ed566c", ChannelType = ChannelType.SMS, Target = "05544405806"},
            new() { UserId = 1005, WatchedAddress = "0xa9bffc8b3fbbd7164f5e8d8ce86a328a611c2f81", ChannelType = ChannelType.SMS, Target = "05544405806"},
            new() { UserId = 1006, WatchedAddress = "0x92746cb8c986054d35746753107521cc6252a61a", ChannelType = ChannelType.SMS, Target = "05544405806"},
            new() { UserId = 1006, WatchedAddress = "0x6cc9397c3b38739dacbfaa68ead5f5d77ba5f455", ChannelType = ChannelType.PushNotification, Target = "http://example.com"}
        };

            foreach (var channel in channels)
            {
                await _service.AddNotificationChannelAsync(channel);
            }
        }
    }

}
