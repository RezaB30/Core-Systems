using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;

namespace RadiusR.DB.DomainsCache
{
    public static class DomainsCache
    {
        private static MemoryCache InternalCache = new MemoryCache("DomainsCache");
        private static object ReadLock = new object();
        private static object UpdateLock = new object();

        public static IEnumerable<CachedDomain> GetAllDomains()
        {
            var results = (IEnumerable<CachedDomain>)InternalCache.Get("Domains");
            if (results == null)
            {
                if (Monitor.TryEnter(ReadLock, 10000))
                {
                    try
                    {
                        results = (IEnumerable<CachedDomain>)InternalCache.Get("Domains");
                        if (results != null)
                            return results;
                        UpdateCache();
                        results = (IEnumerable<CachedDomain>)InternalCache.Get("Domains");
                        return results;
                    }
                    finally
                    {
                        Monitor.Exit(ReadLock);
                    }
                }
            }

            return results;
        }

        public static CachedDomain GetDomainByID(int id)
        {
            var all = GetAllDomains();
            return all.FirstOrDefault(d => d.ID == id);
        }

        public static bool UpdateCache()
        {
            if (Monitor.TryEnter(UpdateLock, 3000))
            {
                try
                {
                    using (RadiusREntities db = new RadiusREntities())
                    {
                        InternalCache.Set("Domains", db.Domains.Select(d => new CachedDomain()
                        {
                            ID = d.ID,
                            AccessMethod = d.AccessMethod,
                            Name = d.Name,
                            SubscriberNoPrefix = d.SubscriberNoPrefix,
                            UsernamePrefix = d.UsernamePrefix,
                            MaxFreezeDuration = d.MaxFreezeDuration,
                            MaxFreezesPerYear = d.MaxFreezesPerYear,
                            TelekomCredential = d.TelekomAccessCredential != null ? new CachedDomain.TelekomCredentials()
                            {
                                OLOPortalCustomerCode = d.TelekomAccessCredential.OLOPortalCustomerCode,
                                OLOPortalPassword = d.TelekomAccessCredential.OLOPortalPassword,
                                OLOPortalUsername = d.TelekomAccessCredential.OLOPortalUsername,
                                XDSLWebServiceCustomerCode = d.TelekomAccessCredential.XDSLWebServiceCustomerCode,
                                XDSLWebServicePassword = d.TelekomAccessCredential.XDSLWebServicePassword,
                                XDSLWebServiceUsername = d.TelekomAccessCredential.XDSLWebServiceUsername,
                                TransitionFTPUsername = d.TelekomAccessCredential.TransitionFTPUsername,
                                TransitionFTPPassword = d.TelekomAccessCredential.TransitionFTPPassword,
                                TransitionOperatorID = d.TelekomAccessCredential.TransitionOperatorID
                            } : null
                        }).ToArray().AsEnumerable(), DateTimeOffset.Now.AddMinutes(15));
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

        public static IEnumerable<CachedDomain> GetTelekomDomains()
        {
            return GetAllDomains().Where(d => d.TelekomCredential != null).ToArray();
        }

        public static bool HasAnyTelekomDomains
        {
            get
            {
                return GetTelekomDomains().Any();
            }
        }
    }
}
