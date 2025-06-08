using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Infrastructure.Services.Redis
{
    public interface IRedisService
    {
        /// <summary>
        /// HasKeyAsync -> Bu key daha önce kullanıldı mı?
        /// SaveResponseAsync -> Sonucu cache'e kaydet
        /// GetResponseAsync -> Cache'den sonucu al
        Task<bool> HasKeyAsync(string key);
        Task SaveResponseAsync(string key, object response, TimeSpan? tt1 = null);
        Task<T> GetResponseAsync<T>(string key);
    }
}
