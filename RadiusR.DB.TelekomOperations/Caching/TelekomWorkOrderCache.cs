using RadiusR.DB.TelekomOperations.Wrappers;
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
                        var dbWorkOrders = db.TelekomWorkOrders.Where(two => two.IsOpen).OrderBy(two => two.CreationDate)
                            .PrepareForStatusCheck().ToArray();

                        var resultsList = new ConcurrentBag<CachedTelekomWorkOrder>();
                        Parallel.ForEach(dbWorkOrders, (current) =>
                        {
                            var statusClient = new TTWorkOrderClient();
                            resultsList.Add(statusClient.GetWorkOrderState(current));
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

        public static IEnumerable<CachedOutgoingTransition> GetOutgoingList()
        {
            var cachedList = internalCache.Get("CachedOutgoingList") as IEnumerable<CachedOutgoingTransition>;
            if (cachedList != null)
                return cachedList;

            if (Monitor.TryEnter(_cacheLock, TimeSpan.FromSeconds(200)))
            {
                try
                {
                    cachedList = internalCache.Get("CachedOutgoingList") as IEnumerable<CachedOutgoingTransition>;
                    if (cachedList != null)
                        return cachedList;
                    var resultList = new ConcurrentBag<CachedOutgoingTransition>();
                    var cachedOperators = DomainsCache.TransitionOperatorsCache.GetAllOperators();
                    foreach (var telekomValidDomain in DomainsCache.DomainsCache.GetTelekomDomains())
                    {
                        var transitionServiceClient = new RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionApplicationClient(telekomValidDomain.TelekomCredential.XDSLWebServiceUsernameInt, telekomValidDomain.TelekomCredential.XDSLWebServicePassword, telekomValidDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt);
                        var serviceResults = transitionServiceClient.GetIncomingTransitions();
                        if (serviceResults.InternalException != null)
                        {
                            continue;
                        }
                        Parallel.ForEach(serviceResults.Data, (current) =>
                        {
                            var statusClient = new RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionApplicationClient(telekomValidDomain.TelekomCredential.XDSLWebServiceUsernameInt, telekomValidDomain.TelekomCredential.XDSLWebServicePassword, telekomValidDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt);
                            var statusResult = statusClient.GetTransitionStatus(new RezaB.TurkTelekom.WebServices.TTChurnApplication.TransitionStatusRequest()
                            {
                                TransactionID = current.TransactionID,
                                XDSLNo = current.ConnectionInfo.XDSLNo
                            });

                            var operatorName = current.ReceiverOperator?.ReceiverISPCode;
                            if (statusResult.InternalException == null)
                            {
                                operatorName = cachedOperators.FirstOrDefault(op => op.Username == statusResult.Data.ReceiverOperatorName)?.DisplayName ?? statusResult.Data.ReceiverOperatorName;
                            }

                            resultList.Add(new CachedOutgoingTransition(telekomValidDomain.ID, current.CreationDate, current.TransactionID, current.ConnectionInfo?.XDSLNo, operatorName, current.IndividualCustomer != null ? new CachedOutgoingTransition.IndividualCustomerInfo(current.IndividualCustomer.FirstName, current.IndividualCustomer.LastName, current.IndividualCustomer.TCKNo) : null, current.CorporateCustomer != null ? new CachedOutgoingTransition.CorporateCustomerInfo(current.CorporateCustomer.CompanyTitle, current.CorporateCustomer.ExecutiveTCKNo, current.CorporateCustomer.TaxNo) : null));
                        });
                    }
                    var finalResults = resultList.ToArray();
                    internalCache.Set("CachedOutgoingList", finalResults, GetCachePolicy());

                    return finalResults;
                }
                finally
                {
                    Monitor.Exit(_cacheLock);
                }
            }

            return Enumerable.Empty<CachedOutgoingTransition>();
        }

        public static void ClearOutgoingListCache()
        {
            internalCache.Remove("CachedOutgoingList");
        }
    }
}
