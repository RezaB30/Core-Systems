using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RadiusR.DB.TelekomOperations.Caching
{
    public class TelekomWorkOrderCache
    {
        private TelekomWorkOrderCache() { }

        private static MemoryCache internalCache = new MemoryCache("TelekomWorkOrderCache");
        private static object _cacheLock = new object();

        private static CacheItemPolicy GetCachePolicy()
        {
            return new CacheItemPolicy()
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10)
            };
        }

        public static IEnumerable<CachedTelekomWorkOrder> GetCachedList()
        {
            var cachedList = internalCache.Get("CachedList") as IEnumerable<CachedTelekomWorkOrder>;
            if (cachedList != null)
                return cachedList;

            if (Monitor.TryEnter(_cacheLock, TimeSpan.FromSeconds(100)))
            {
                try
                {
                    cachedList = internalCache.Get("CachedList") as IEnumerable<CachedTelekomWorkOrder>;
                    if (cachedList != null)
                        return cachedList;

                    using (RadiusREntities db = new RadiusREntities())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        var dbWorkOrders = db.TelekomWorkOrders.Where(two => two.IsOpen).OrderBy(two => two.CreationDate).Select(two => new {
                            ID = two.ID,
                            DomainId = two.Subscription.DomainID,
                            TelekomCustomerCode = two.Subscription.SubscriptionTelekomInfo != null ? two.Subscription.SubscriptionTelekomInfo.TTCustomerCode : (long?)null,
                            QueueNo = two.QueueNo,
                            ManagementCode = two.ManagementCode,
                            ProvinceCode = two.ProvinceCode
                        }).ToArray();

                        var resultsList = new ConcurrentBag<CachedTelekomWorkOrder>();
                        Parallel.ForEach(dbWorkOrders, (current) =>
                        {
                            var currentDomain = DomainsCache.DomainsCache.GetDomainByID(current.DomainId);
                            if (currentDomain == null || currentDomain.TelekomCredential == null)
                            {
                                resultsList.Add(new CachedTelekomWorkOrder(current.ID, (short)RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState.Unknown));
                            }
                            else
                            {
                                var serviceClient = new RezaB.TurkTelekom.WebServices.TTApplication.TTApplicationServiceClient(currentDomain.TelekomCredential.XDSLWebServiceUsernameInt, currentDomain.TelekomCredential.XDSLWebServicePassword, current.TelekomCustomerCode ?? currentDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt);
                                var response = serviceClient.TraceRegistration(current.ProvinceCode.Value, current.ManagementCode.Value, current.QueueNo.Value);
                                if (response.InternalException != null)
                                {
                                    resultsList.Add(new CachedTelekomWorkOrder(current.ID, (short)RezaB.TurkTelekom.WebServices.TTApplication.RegistrationState.Unknown));
                                }
                                else
                                {
                                    resultsList.Add(new CachedTelekomWorkOrder(current.ID, (short)response.Data.State));
                                }
                            }
                        });
                        var finalResults = resultsList.ToArray();
                        internalCache.Set("CachedList", finalResults, GetCachePolicy());

                        return finalResults;
                    }
                }
                finally
                {
                    Monitor.Exit(_cacheLock);
                }
            }

            return Enumerable.Empty<CachedTelekomWorkOrder>();
        }
    }
}
