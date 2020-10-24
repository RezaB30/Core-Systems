using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RadiusR.DB.Enums;
using RezaB.TurkTelekom.WebServices.TTApplication;

namespace RadiusR.DB.DomainsCache
{
    public static class TelekomTariffsCache
    {
        private static MemoryCache InternalCache = new MemoryCache("TelekomTariffsCache");
        private static object ReadLock = new object();
        private static object UpdateLock = new object();
        private static DomainInfrastructure[] ValidAccessMethods = new[] { DomainInfrastructure.AlSat, DomainInfrastructure.VAE };

        public static IEnumerable<CachedTelekomTariff> GetAllTariffs(CachedDomain domain)
        {
            // check domain for valid tariffs
            if (!domain.AccessMethod.HasValue || !Enum.IsDefined(typeof(DomainInfrastructure), (int)domain.AccessMethod.Value) || !ValidAccessMethods.Contains((DomainInfrastructure)domain.AccessMethod.Value))
            {
                return Enumerable.Empty<CachedTelekomTariff>();
            }

            var results = (IEnumerable<CachedTelekomTariff>)InternalCache.Get("TelekomTariffs_" + domain.AccessMethod.Value);
            if (results == null)
            {
                if (Monitor.TryEnter(ReadLock, 10000))
                {
                    try
                    {
                        results = (IEnumerable<CachedTelekomTariff>)InternalCache.Get("TelekomTariffs_" + domain.AccessMethod.Value);
                        if (results != null)
                            return results;
                        if (UpdateCache(domain))
                        {
                            results = (IEnumerable<CachedTelekomTariff>)InternalCache.Get("TelekomTariffs_" + domain.AccessMethod.Value);
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

        public static CachedTelekomTariff GetSpecificTariff(CachedDomain domain, int packetCode, int tariffCode)
        {
            // check domain for valid tariffs
            if (!domain.AccessMethod.HasValue || !Enum.IsDefined(typeof(DomainInfrastructure), (int)domain.AccessMethod.Value) || !ValidAccessMethods.Contains((DomainInfrastructure)domain.AccessMethod.Value))
            {
                return null;
            }

            var all = GetAllTariffs(domain);
            if (all == null)
                return null;
            return all.FirstOrDefault(t => t.PacketCode == packetCode && t.TariffCode == tariffCode);
        }

        private static bool UpdateCache(CachedDomain domain)
        {
            // check domain for valid tariffs
            if (!domain.AccessMethod.HasValue || !Enum.IsDefined(typeof(DomainInfrastructure), (int)domain.AccessMethod.Value) || !ValidAccessMethods.Contains((DomainInfrastructure)domain.AccessMethod.Value) || domain.TelekomCredential == null)
            {
                return false;
            }

            if (Monitor.TryEnter(UpdateLock, 3000))
            {
                try
                {
                    using (RadiusREntities db = new RadiusREntities())
                    {
                        db.Configuration.AutoDetectChangesEnabled = false;
                        var allSpeedNames = db.TelekomTariffs.ToArray().ToDictionary(t => t.SpeedCode, t => t.Name);

                        var serviceClient = new TTApplicationServiceClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword, domain.TelekomCredential.XDSLWebServiceCustomerCodeInt);
                        var telekomTariffs = serviceClient.ListTariffs((ApplicationType)domain.AccessMethod.Value);
                        if (telekomTariffs.InternalException != null)
                        {
                            return false;
                        }

                        InternalCache.AddOrGetExisting("TelekomTariffs_" + domain.AccessMethod.Value, telekomTariffs.Data.Select(tt => new CachedTelekomTariff()
                        {
                            MonthlyStaticFee = tt.MonthlyStaticFee,
                            PacketCode = tt.Code,
                            SpeedCode = tt.Speed,
                            SpeedDetails = tt.SpeedProfile,
                            TariffCode = tt.TypeCode,
                            TariffName = tt.Name,
                            XDSLType = tt.XDSLType,
                            SpeedName = allSpeedNames.ContainsKey(tt.Speed) ? allSpeedNames[tt.Speed] : tt.SpeedProfile
                        }).ToArray().AsEnumerable(), DateTimeOffset.Now.AddDays(1));
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
