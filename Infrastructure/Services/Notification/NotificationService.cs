using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Options;
using Application.ServicesImpl;
using Microsoft.Extensions.Options;
using Persistence.Repositories;

namespace Infrastructure.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Domain.Entities.Notification> _notificationRepository;
        private readonly decimal _minEthAmount;

        public NotificationService(IRepository<Domain.Entities.Notification> notificationRepository, IOptions<NotificationOptions> options)
        {
            _notificationRepository = notificationRepository;
            _minEthAmount = options.Value.MinEthAmount;

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
                ErrorMessage = "-"
            };

            await _notificationRepository.AddAsync(log);
            Console.WriteLine($"[BİLDİRİM] => [Message]: {log.Message} => [Kanal]:{channel.ChannelType.ToString()} => [UserId]:{log.UserId} ");
        }

        public async Task SimulateNotificationsForTransactionsAsync(List<ExternalTransactionDto> transactions, List<Domain.Entities.NotificationChannel> channels)
        {
            foreach (var tx in transactions)
            {
                if(tx.Amount < _minEthAmount)
                {
                    continue; // Minimum ETH miktarını karşılamayan işlemleri atla
                }

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
