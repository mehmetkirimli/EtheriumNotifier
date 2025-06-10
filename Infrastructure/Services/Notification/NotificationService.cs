using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.ServicesImpl;
using Persistence.Repositories;

namespace Infrastructure.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Domain.Entities.Notification> _notificationRepository;
        private readonly IRepository<Domain.Entities.NotificationChannel> _notificationChannelRepository;

        public NotificationService(IRepository<Domain.Entities.Notification> notificationRepository, IRepository<Domain.Entities.NotificationChannel> notificationChannelRepository)
        {
            _notificationRepository = notificationRepository;
            _notificationChannelRepository = notificationChannelRepository;
        }

        public async Task LogNotificationAsync(ExternalTransactionDto tx, Domain.Entities.NotificationChannel channel)
        {
            var log = new Domain.Entities.Notification
            {
                UserId = channel.UserId,
                ChannelType = channel.ChannelType,
                Target = channel.Target,
                Message = $"{tx.Amount} ETH transfer received from address {tx.From} to address {tx.To} ({tx.ProcessingTime})",
                SentAt = DateTime.UtcNow,
                TransactionHash = tx.TransactionHash,
                IsSuccess = true,
                ErrorMessage = null
            };

            await _notificationRepository.AddAsync(log);
            Console.WriteLine($"[BİLDİRİM]: Kanal:{channel.ChannelType.ToString()}, Tx:{tx.TransactionHash} , Message: {log.Message}");
        }

        public async Task SimulateNotificationsForTransactionsAsync(List<ExternalTransactionDto> transactions, List<Domain.Entities.NotificationChannel> channels)
        {
            foreach (var tx in transactions)
            {
                foreach (var channel in channels)
                {
                    if (channel.WatchedAddress == tx.To || channel.WatchedAddress == tx.From)
                    {
                        await LogNotificationAsync(tx, channel);
                    }
                }
            }
        }

    }
}
