using System;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace Infrastructure.CacheContent
{
    public sealed class MemcachedContext : ICacheContext
    {
        private static readonly MemcachedClient MemcachedClient  = new MemcachedClient();

        public override T Get<T>(string key)
        {
            return MemcachedClient.Get<T>(key);
        }

        public override bool Set<T>(string key, T t, DateTime expire)
        {
            return MemcachedClient.Store(StoreMode.Set, key, t, expire);
        }

        public override bool Remove(string key)
        {
            return MemcachedClient.Remove(key);
        }
    }
}