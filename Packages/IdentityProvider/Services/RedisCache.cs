using Barracuda.Indentity.Provider.Interfaces;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Barracuda.Indentity.Provider.Services
{
    public class RedisCache : IRedisCache
    {
        private Lazy<ConnectionMultiplexer> lazyConnection;
        private readonly ISettingsRedis _settings;
      
        public RedisCache(
            ISettingsRedis settings
            )
        {
            _settings = settings;
       
            lazyConnection = new Lazy<ConnectionMultiplexer>
               (() =>
               {
                   return ConnectionMultiplexer.Connect(_settings.RedisCacheConnection);
               });
        }

        private IDatabase Cache
        {
            get
            {
                return lazyConnection.Value.GetDatabase();
            }
        }

        public async Task<string> GetSringValue(string id)
        {
            string value = null;

            var redis = await Cache.StringGetAsync(id);
            if (!redis.IsNullOrEmpty)
            {
                value = redis;
            }

            return value;
        }

        public async Task SetStringValue(string id, string value)
        {
            await Cache.StringSetAsync(id, value);
        }

    }
}
