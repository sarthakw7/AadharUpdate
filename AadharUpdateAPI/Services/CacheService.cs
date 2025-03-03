using Microsoft.Extensions.Caching.Memory;

namespace AadharUpdateAPI.Services
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan? expirationTime = null);
        void Remove(string key);
    }

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _defaultExpirationTime = TimeSpan.FromMinutes(45); // Default 45 minutes

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? Get<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T? value);
            return value;
        }

        public void Set<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            var memoryCacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime ?? _defaultExpirationTime,
                SlidingExpiration = TimeSpan.FromMinutes(15) // Refresh expiration on access
            };

            _memoryCache.Set(key, value, memoryCacheEntryOptions);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
} 