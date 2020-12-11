using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.Threading;

namespace RadiusR.DB
{
    public partial class RadiusREntities
    {
        private static object _onlineSubscriptionCountLock = new object();
        public int? OnlineSubscriptionCount
        {
            get
            {
                var cachedValue = MemoryCache.Default.Get("Context_OnlineSubscriptionCount") as int?;
                if (!cachedValue.HasValue)
                {
                    if (Monitor.TryEnter(_onlineSubscriptionCountLock, TimeSpan.FromSeconds(5)))
                    {
                        try
                        {

                            cachedValue = MemoryCache.Default.Get("Context_OnlineSubscriptionCount") as int?;
                            if (cachedValue.HasValue)
                                return cachedValue;
                            cachedValue = RadiusAccountings.Where(ra => !ra.StopTime.HasValue).Count();
                            MemoryCache.Default.Set("Context_OnlineSubscriptionCount", cachedValue, DateTimeOffset.Now.AddSeconds(15));
                        }
                        finally
                        {
                            Monitor.Exit(_onlineSubscriptionCountLock);
                        }
                    }
                }
                return cachedValue;
            }
        }
    }
}
