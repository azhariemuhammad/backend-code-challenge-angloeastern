using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ShipManagement.Interfaces;
namespace ShipManagement.Services
{
    public class RedisService : IRedisCacheService
    {
        private readonly IDistributedCache _distributedCache;

        public RedisService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _distributedCache.GetStringAsync(key);

            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
            };

            var jsonData = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, jsonData, options);
        }
    }
}