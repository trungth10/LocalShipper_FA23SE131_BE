using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Reso.Sdk.Core.Extension;

namespace LocalShipper.Service.Caches
{
    public static class CacheManager
    {
        public static async System.Threading.Tasks.Task<T> GetObjectAsync<T>(IMemoryCache memoryCache = null,
            IDistributedCache distributedCache = null,
            string key = null) where T : class
        {
            T rs = null;
            if (memoryCache != null && memoryCache.TryGetValue(key, out rs))
            {
                return rs;
            }

            if (distributedCache != null)
            {
                try
                {
                    rs = await distributedCache.GetAsync<T>(key);
                }
                catch
                {
                    //do nothing
                }
            }

            if (memoryCache != null && rs != null)
            {
                memoryCache.Set(key, rs);

            }
            return rs;
        }

        public static async System.Threading.Tasks.Task SetObjectAsync(TimeSpan timeOut,
            IMemoryCache memoryCache = null,
            IDistributedCache distributedCache = null, string key = null, object obj = null)
        {
            var serializerResponse = JsonConvert.SerializeObject(obj, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });

            if (memoryCache != null)
            {
                memoryCache.Set(key, obj);
            }

            if (distributedCache != null)
            {
                try
                {
                    await distributedCache.SetStringAsync(key, serializerResponse, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = timeOut
                    });
                }
                catch
                {
                    //do nothing
                }
            }
        }

        public static async System.Threading.Tasks.Task RemoveObjectAsync(IMemoryCache memoryCache = null,
            IDistributedCache distributedCache = null, string key = null, object obj = null)
        {
            if (memoryCache != null)
            {
                memoryCache.Remove(key);
            }

            if (distributedCache != null)
            {
                try
                {
                    await distributedCache.RemoveAsync(key);
                }
                catch
                {
                    //do nothing
                }
            }
        }
        public static List<string> GetAllkeys(IConnectionMultiplexer connectionMultiplexe,
            IConfiguration configuration)
        {
            List<string> listKeys = new List<string>();

            var keys = connectionMultiplexe.GetServer(configuration["Endpoint:RedisEndpoint"]).Keys();
            listKeys.AddRange(keys.Select(key => (string)key).ToList());
            return listKeys;
        }
    }
}

