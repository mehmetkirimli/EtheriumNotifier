using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Domain.Entities;

namespace Application.ServicesImpl
{
    public interface INotificationService
    {
        Task LogNotificationAsync(ExternalTransactionDto tx, NotificationChannel channel);
        Task SimulateNotificationsForTransactionsAsync(List<ExternalTransactionDto> transactions, List<NotificationChannel> channels);
        Task<List<NotificationLogDto>> GetNotificationLogsAsync(NotificationLogFilterRequestDto dto);
    }
}
