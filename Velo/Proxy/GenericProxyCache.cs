using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Runtime.Caching;

namespace Proxy
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class GenericProxyCache : IGenericProxyCache
    {

        private ObjectCache cache = MemoryCache.Default;
        private DateTimeOffset dt_default = ObjectCache.InfiniteAbsoluteExpiration;


        public T Get<T>(string CacheItemName)
        {
            var ItemValue = cache.Get(CacheItemName);

            if (ItemValue == null || !cache.Contains(CacheItemName))
            {
                var instance = Activator.CreateInstance(typeof(T), CacheItemName);
                cache.Add(CacheItemName, instance, dt_default);
            }

            return (T)cache.Get(CacheItemName);

        }

        public T Get<T>(string CacheItemName, double dt_seconds)
        {
            var ItemValue = cache.Get(CacheItemName);

            if (ItemValue == null || !cache.Contains(CacheItemName))
            {

                DateTimeOffset dt_new = dt_default.AddSeconds(dt_seconds);
                var instance = Activator.CreateInstance(typeof(T), CacheItemName);
                cache.Add(CacheItemName, instance, dt_default);
            }

            return (T)cache.Get(CacheItemName);
        }

        public T Get<T>(string CacheItemName, DateTimeOffset dt)
        {
            var ItemValue = cache.Get(CacheItemName);

            if (ItemValue == null || !cache.Contains(CacheItemName))
            {
                var instance = Activator.CreateInstance(typeof(T), CacheItemName);
                cache.Add(CacheItemName, instance, dt_default);
            }

            return (T)cache.Get(CacheItemName);
        }
    }
}
