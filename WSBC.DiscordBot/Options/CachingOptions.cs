using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSBC.Discord
{
    class CachingOptions
    {
        public TimeSpan DataCacheLifetime { get; set; } = TimeSpan.FromSeconds(10);
    }
}
