using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RadiusR.DB.DomainsCache
{
    public static class TransitionOperatorsCache
    {
        private static MemoryCache InternalCache = new MemoryCache("TransitionOperatorsCache");
        private static object ReadLock = new object();
        private static object UpdateLock = new object();

        public static IEnumerable<CachedTransitionOperator> GetAllOperators()
        {

            var results = (IEnumerable<CachedTransitionOperator>)InternalCache.Get("AllOperators");
            if (results == null)
            {
                if (Monitor.TryEnter(ReadLock, 4000))
                {
                    try
                    {
                        results = (IEnumerable<CachedTransitionOperator>)InternalCache.Get("AllOperators");
                        if (results != null)
                            return results;
                        if (UpdateCache())
                        {
                            results = (IEnumerable<CachedTransitionOperator>)InternalCache.Get("AllOperators");
                            return results;
                        }
                    }
                    finally
                    {
                        Monitor.Exit(ReadLock);
                    }
                }
            }

            return results;
        }

        public static CachedTransitionOperator GetSpecificOperator(int id)
        {
            var all = GetAllOperators();
            if (all == null)
                return null;
            return all.FirstOrDefault(to => to.ID == id);
        }

        private static bool UpdateCache()
        {
            if (Monitor.TryEnter(UpdateLock, 3000))
            {
                try
                {
                    using (RadiusREntities db = new RadiusREntities())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        var allOperators = db.TransitionOperators.ToArray().Select(to => new CachedTransitionOperator()
                        {
                            DisplayName = to.DisplayName,
                            ID = to.ID,
                            Username = to.TelekomUsername,
                            RemoteFolders = to.RemoteFolder.Split('\t')
                        }).ToArray();

                        InternalCache.AddOrGetExisting("AllOperators", allOperators.AsEnumerable(), DateTimeOffset.Now.AddMinutes(15));
                    }
                    return true;
                }
                finally
                {
                    Monitor.Exit(UpdateLock);
                }
            }
            return false;
        }

        public static void ClearCache()
        {
            var allKeys = InternalCache.Select(mc => mc.Key).ToArray();
            foreach (var key in allKeys)
            {
                InternalCache.Remove(key);
            }
        }
    }
}
