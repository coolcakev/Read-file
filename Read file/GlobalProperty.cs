using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_file
{
    public static class GlobalProperty
    {
        private static SimpleMemoryCache _memoryCache;
        public static SimpleMemoryCache MemoryCache
        {
            get
            {
                if (_memoryCache == null)
                {
                    _memoryCache = new SimpleMemoryCache();
                }
                return _memoryCache;
            }
        }
    }
}
