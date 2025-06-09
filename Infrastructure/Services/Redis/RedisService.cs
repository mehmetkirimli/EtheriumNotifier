using System.Text.Json;
using StackExchange.Redis;

namespace Infrastructure.Services.Redis
{
    public class RedisService : IRedisService
    {
        private readonly StackExchange.Redis.IDatabase _redis;

        public RedisService(IConnectionMultiplexer connectionMultiplexer)
        {
            _redis = connectionMultiplexer.GetDatabase();
        }

        public async Task<bool> HasKeyAsync(string key)
        {
            return await _redis.KeyExistsAsync(key);
        }
        public async Task SaveResponseAsync(string key, object response, TimeSpan? tt1 = null)
        {
            string json = JsonSerializer.Serialize(response);
            await _redis.StringSetAsync(key, json, tt1 ?? TimeSpan.FromHours(2));
        }

        public async Task<T> GetResponseAsync<T>(string key)
        {
            var value = await _redis.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default(T);
        }

        public async Task AddHashToMinuteSetAsync(string hash, DateTime dateTimeUtc)
        {
            // Key: Tx-Minute:2024-06-09-13:45
            string setKey = $"Tx-Minute:{dateTimeUtc:yyyy-MM-dd-HH:mm}";
            await _redis.SetAddAsync(setKey, hash); // StackExchange.Redis SetAdd
        }
    }
}
