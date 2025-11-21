using Microsoft.Extensions.Caching.Memory;

namespace kendo_northwind_pg.Data.Repositories
{
    public interface IUserDataCache
    {
        IList<T> GetOrCreateList<T>(string userKey, string logicalName, Func<IList<T>> factory, TimeSpan ttl, bool sliding);
        void Invalidate(string userKey, string logicalName);
        bool TryGetList<T>(string userKey, string logicalName, out List<T> list);
    }

    public class UserDataCache : IUserDataCache
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<UserDataCache> _logger;

        public UserDataCache(IMemoryCache cache, ILogger<UserDataCache> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public bool TryGetList<T>(string userKey, string logicalName, out List<T> list)
        {
            var key = BuildKey(userKey, logicalName);
            return _cache.TryGetValue(key, out list);
        }

        public IList<T> GetOrCreateList<T>(string userKey, string logicalName, Func<IList<T>> factory, TimeSpan ttl, bool sliding)
        {
            var key = BuildKey(userKey, logicalName);
            if (_cache.TryGetValue(key, out IList<T> list))
            {
                return list;
            }

            list = factory();

            var opts = new MemoryCacheEntryOptions();
            if (sliding)
                opts.SetSlidingExpiration(ttl);
            else
                opts.SetAbsoluteExpiration(ttl);

            opts.RegisterPostEvictionCallback((k, v, reason, state) =>
            {
                _logger.LogInformation("Evicted user cache key {Key} due to {Reason}", k, reason);
            });

            _cache.Set(key, list, opts);
            return list;
        }

        public void Invalidate(string userKey, string logicalName)
        {
            var key = BuildKey(userKey, logicalName);
            _cache.Remove(key);
        }

        private static string BuildKey(string userKey, string logicalName) => $"user:{userKey}:{logicalName}";
    }
}