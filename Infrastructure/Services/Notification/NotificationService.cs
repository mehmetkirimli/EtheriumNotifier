using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Options;
using Application.ServicesImpl;
using AutoMapper;
using Infrastructure.Services.Etherium;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistence.Repositories;

namespace Infrastructure.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Domain.Entities.Notification> _notificationRepository;
        private readonly decimal _minEthAmount;
        private readonly IMapper _mapper;
        private readonly ILogger<Domain.Entities.Notification> _logger;

        public NotificationService(IRepository<Domain.Entities.Notification> notificationRepository, IOptions<NotificationOptions> options , IMapper mapper, ILogger<Domain.Entities.Notification> logger)
        {
            _notificationRepository = notificationRepository;
            _minEthAmount = options.Value.MinEthAmount;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<NotificationLogDto>> GetNotificationLogsAsync(NotificationLogFilterRequestDto dto)
        {
            if(dto.FromDate.HasValue && dto.ToDate.HasValue) 
            { 
                if((dto.ToDate - dto.FromDate).Value.TotalDays > 30) 
                { 
                    throw new ArgumentException("Tarih aralığı maksimum 30 gün olmalıdır!");
                }
            }

            var all = await _notificationRepository.GetAllAsync();
            var query = all.AsQueryable();

            if (dto.UserId.HasValue)
                query = query.Where(x => x.UserId == dto.UserId.Value);

            if (dto.Channels != null && dto.Channels.Any())
                query = query.Where(x => dto.Channels.Contains(x.ChannelType));

            if (dto.FromDate.HasValue)
                query = query.Where(x => x.SentAt >= dto.FromDate.Value);

            if (dto.ToDate.HasValue)
                query = query.Where(x => x.SentAt <= dto.ToDate.Value);

            var filteredEntities = query.OrderByDescending(x => x.SentAt).ToList();
            var result = _mapper.Map<List<NotificationLogDto>>(filteredEntities);

            return result;
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
            _logger.LogInformation("Get Transaction and Save process is succesfully .");
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
