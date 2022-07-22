using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_file
{
    public class SimpleMemoryCache
    {
        private MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public TItem Get<TItem>(object key)
        {
            if (_cache.TryGetValue(key, out TItem cacheEntry))
            {
                return cacheEntry;
            }
            return default(TItem);
        }
        public void Set<TItem>(object key, TItem item)
        {
            _cache.Set(key, item);
        }       
    }
}
