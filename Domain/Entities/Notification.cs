using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Entities
{
    /// <summary>
    /// Simüle edilecek olan bildirimleri loglamak için kullanılan sınıf
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int UserId { get; set; }
        public ChannelType ChannelType { get; set; } 
        public string Target { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public string TransactionHash { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /*/
     * Note: Bir transaction kaydedildiği vakit
     * Notification Kanallarına Bildirim Gönderilecek (Notification Tipinde)
     * Notification entity'si ile log kaydı oluşturucaz . 
     */
}
