using System.Text.Json;
using StackExchange.Redis;
using StoreCore.ServicesContract;

namespace StoreService.Service.Caches
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase database;
        public CacheService(IConnectionMultiplexer redis)
        {
            database = redis.GetDatabase();
        }


        public async Task<string> GetCacheKeyAsync(string key)
        {
            var cacheResponse = await database.StringGetAsync(key);
            if (cacheResponse.IsNullOrEmpty)
                return null;
            return cacheResponse.ToString();
        }
        public async Task SetCacheKeyAsync(string key, object response, TimeSpan? expireTime = null)
        {
            if (response == null)
                return;
            var options =new JsonSerializerOptions() { AllowTrailingCommas = true, WriteIndented = true };
            await database.StringSetAsync(key, JsonSerializer.Serialize(response,options), expireTime);
        }
    }
}
