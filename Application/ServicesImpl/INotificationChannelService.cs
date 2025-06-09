using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Domain.Entities;
using Domain.Enums;

namespace Application.ServicesImpl
{
    public interface INotificationChannelService
    {
        public Task<NotificationChannel> AddNotificationChannelAsync(CreateNotificationChannelRequestDto dto);
        public Task<NotificationChannel> UpdateNotificationChannelAsync(CreateNotificationChannelRequestDto dto);
        public Task<NotificationChannel> DeleteNotificationChannelAsync(Guid channelId);
        public Task<List<NotificationChannel>> GetNotificationChannelsAsync(ChannelType? channelType , int? userId );
        public Task<NotificationChannel> GetNotificationChannelByIdAsync(Guid channelId);
    }
}
