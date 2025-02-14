﻿using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.ModelExtentions
{
    public static class SubscriptionUtilities
    {
        /// <summary>
        /// Gets tariff cost in a specific billing period.
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="usage">Subscription data usage (uploda/download).</param>
        /// <param name="currentPeriod">The period to get fee for.</param>
        /// <param name="upToSpecificDate">If not null will limit cost up to this date.</param>
        /// <returns></returns>
        public static decimal? GetCurrentTariffCost(this Subscription subscription, IEnumerable<DailyUsageInfo> usage, BillingPeriod currentPeriod, DateTime? upToSpecificDate = null)
        {
            // calculate partiality
            var partiality = currentPeriod.GetPartiality(upToSpecificDate);
            if (partiality == null)
                return null;

            // base tariff fee
            var baseTariffFee = subscription.Service.Price;

            // for smart quota
            if (subscription.Service.QuotaType == (short)QuotaType.SmartQuota)
            {
                var periodUsage = usage.Where(u => u.Date >= currentPeriod.StartDate && u.Date < currentPeriod.EndDate).Select(u => u.DownloadBytes + u.UploadBytes).DefaultIfEmpty(0).Sum();
                if (periodUsage > subscription.Service.BaseQuota)
                {
                    var proportionalAddedPrice = (periodUsage - (subscription.Service.BaseQuota ?? 0)) * (QuotaSettings.QuotaUnitPrice / (decimal)QuotaSettings.QuotaUnit);
                    var proportionaltariffPrice = baseTariffFee + proportionalAddedPrice;
                    if (proportionaltariffPrice > subscription.Service.SmartQuotaMaxPrice)
                    {
                        proportionaltariffPrice = subscription.Service.SmartQuotaMaxPrice.Value;
                    }

                    return Math.Round(proportionaltariffPrice * partiality.GeneralPartiality, 2);
                }
            }
            // for non-smart
            return Math.Round(baseTariffFee * partiality.GeneralPartiality, 2);
        }
        /// <summary>
        /// Gets the partiality of the current period.
        /// </summary>
        /// <param name="period">The period to get the partiality of.</param>
        /// <param name="upToSpecificDate">If not null will limit the calculation up to this date.</param>
        /// <returns></returns>
        public static PartialityInfo GetPartiality(this BillingPeriod period, DateTime? upToSpecificDate = null)
        {
            if (upToSpecificDate != null)
            {
                upToSpecificDate = upToSpecificDate.Value.Date;
                if (upToSpecificDate < period.StartDate)
                    return null;
                if (upToSpecificDate > period.EndDate)
                    upToSpecificDate = null;
            }
            var periodDays = (period.EndDate.Date - period.StartDate.Date).Days;
            var currentDays = ((upToSpecificDate ?? period.EndDate).Date - (period.StartDate.Date)).Days;
            if (currentDays < 0)
                return null;

            var generalPartiality = (decimal)currentDays / (decimal)DateTime.DaysInMonth(period.StartDate.Year, period.StartDate.Month);

            return new PartialityInfo()
            {
                GeneralPartiality = generalPartiality
            };
        }
        /// <summary>
        /// Gets the amount of remaining quota in a specific period.
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="period">The period to get the remaining quota for.</param>
        /// <param name="db">Data entities.</param>
        /// <returns></returns>
        public static long? GetRemainingQuota(this Subscription subscription, BillingPeriod period, RadiusREntities db)
        {
            var usage = subscription.GetPeriodUsageInfo(period, db);
            if (usage == null)
                return null;
            var quota = subscription.GetPeriodQuota(period, db);
            if (!quota.HasValue)
                return null;

            return Math.Max(0, quota.Value - usage.Total);
        }
        /// <summary>
        /// Gets available quota in a specific period.
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="period">The period to get quota for.</param>
        /// <param name="db">Data entities.</param>
        /// <returns></returns>
        public static long? GetPeriodQuota(this Subscription subscription, BillingPeriod period, RadiusREntities db)
        {
            var startDate = period.StartDate;
            var endDate = period.EndDate;

            if (subscription.Service.QuotaType.HasValue)
            {
                if (subscription.Service.BaseQuota.HasValue)
                {
                    var results = subscription.Service.BaseQuota.Value;
                    results += db.SubscriptionQuotas.Where(sq => sq.SubscriptionID == subscription.ID && sq.AddDate >= startDate && sq.AddDate < endDate).Select(sq => sq.Amount).DefaultIfEmpty(0).Sum();
                    return results;
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Gets the amount of usage (upload/download) for a specific period.
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="period">The period to get usage for.</param>
        /// <param name="db">Data entities.</param>
        /// <returns></returns>
        public static UsageInfo GetPeriodUsageInfo(this Subscription subscription, BillingPeriod period, RadiusREntities db)
        {
            var startDate = period.StartDate;
            var endDate = period.EndDate;

            var usage = db.RadiusDailyAccountings.Where(rda => rda.SubscriptionID == subscription.ID).Where(rda => rda.Date >= startDate && rda.Date < endDate).Select(rda => new { Download = rda.DownloadBytes, Upload = rda.UploadBytes }).ToArray();
            return new UsageInfo()
            {
                Download = usage.Any() ? usage.Sum(u => u.Download) : 0,
                Upload = usage.Any() ? usage.Sum(u => u.Upload) : 0
            };
        }

        /// <summary>
        /// Gets the quota and amount based on a specific date int a period.
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="baseDate">The date that billing period will contain. (null = today)</param>
        /// <returns></returns>
        public static QuotaAndUsageInfo GetQuotaAndUsageInfo(this Subscription subscription, DateTime? baseDate = null)
        {
            baseDate = baseDate ?? DateTime.Today;
            var currentPeriod = subscription.GetCurrentBillingPeriod(baseDate, ignoreActivationDate: true);
            if (currentPeriod == null)
                return null;
            using (RadiusREntities db = new RadiusREntities())
            {
                db.Configuration.AutoDetectChangesEnabled = false;
                var startDate = currentPeriod.StartDate;
                var endDate = currentPeriod.EndDate;

                var usageInfo = db.Subscriptions.Where(s => s.ID == subscription.ID).Select(sub => new
                {
                    SubID = sub.ID,
                    Quota = sub.SubscriptionQuotas.Where(q => DbFunctions.TruncateTime(q.AddDate) >= startDate && DbFunctions.TruncateTime(q.AddDate) < endDate).Select(q => q.Amount).DefaultIfEmpty(0).Sum(),
                    Usage = sub.RadiusDailyAccountings.Where(rda => rda.Date >= startDate && rda.Date < endDate).Select(rda => new { Usage = rda.DownloadBytes + rda.UploadBytes, Date = rda.Date }),
                    LastQuota = sub.SubscriptionQuotas.OrderByDescending(q => q.AddDate).FirstOrDefault()
                }).FirstOrDefault();

                return new QuotaAndUsageInfo()
                {
                    PeriodStart = currentPeriod.StartDate,
                    PeriodEnd = currentPeriod.EndDate,
                    PeriodUsage = usageInfo.Usage.Select(u => u.Usage).DefaultIfEmpty(0).Sum(),
                    PeriodQuota = usageInfo.Quota + subscription.Service.BaseQuota ?? 0,
                    LastQuotaChangeDate = usageInfo.LastQuota != null ? usageInfo.LastQuota.AddDate : startDate,
                    LastQuotaAmount = usageInfo.LastQuota != null ? usageInfo.LastQuota.Amount : subscription.Service.BaseQuota ?? 0,
                    LastQuotaUsage = usageInfo.Usage.Where(u => u.Date >= (usageInfo.LastQuota != null ? usageInfo.LastQuota.AddDate : startDate)).Select(u => u.Usage).DefaultIfEmpty(0).Sum()
                };
            }
        }

        /// <summary>
        /// Gets the billing period that contains a specific date.
        /// </summary>
        /// <param name="subscription"></param>
        /// <param name="baseDate">The day that billing period will contain. (null = today)</param>
        /// <param name="ignoreActivationDate">Ignore activation date for updating last allowed date only.</param>
        /// <param name="ignorePreviousBills">Does not check if is currently in an issued bill period so it is faster; use when you know this is the last/current billing period.</param>
        /// <returns></returns>
        public static BillingPeriod GetCurrentBillingPeriod(this Subscription subscription, DateTime? baseDate = null, bool ignoreActivationDate = false, bool ignorePreviousBills = false)
        {
            var currentDate = (baseDate ?? DateTime.Today).Date;
            // if currently have a bill for this period
            if (!ignorePreviousBills)
            {
                var currentBill = subscription.Bills.OrderByDescending(b => b.IssueDate).FirstOrDefault(b => b.PeriodStart <= currentDate && b.PeriodEnd > currentDate);
                if (currentBill != null)
                    return new BillingPeriod()
                    {
                        StartDate = currentBill.PeriodStart.Value,
                        EndDate = currentBill.PeriodEnd.Value,
                        IsBilled = true
                    };
            }

            var results = new BillingPeriod();
            if (subscription.PaymentDay == currentDate.Day)
            {
                results.StartDate = currentDate;
            }
            else if (subscription.PaymentDay < currentDate.Day)
            {
                results.StartDate = new DateTime(currentDate.Year, currentDate.Month, subscription.PaymentDay);
            }
            else
            {
                var oneMonthEarlier = currentDate.AddMonths(-1);
                results.StartDate = new DateTime(oneMonthEarlier.Year, oneMonthEarlier.Month, subscription.PaymentDay);
            }

            results.EndDate = results.StartDate.AddMonths(1);
            // considering activation date
            if (!ignoreActivationDate)
            {
                if (!subscription.IsActive || !subscription.ActivationDate.HasValue || subscription.ActivationDate > results.EndDate)
                    return null;
                if (subscription.ActivationDate.Value.Date > results.StartDate)
                    results.StartDate = subscription.ActivationDate.Value.Date;
            }
            // check last tariff change date for changed billing periods
            if (subscription.LastTariffChangeDate.HasValue && results.StartDate <= subscription.LastTariffChangeDate.Value.Date && results.EndDate > subscription.LastTariffChangeDate.Value.Date)
            {
                if (currentDate >= subscription.LastTariffChangeDate.Value.Date)
                    results.StartDate = subscription.LastTariffChangeDate.Value.Date;
                else
                    results.EndDate = subscription.LastTariffChangeDate.Value.Date;
            }

            return results;
        }

        /// <summary>
        /// Represents a billing period.
        /// </summary>
        public class BillingPeriod
        {
            /// <summary>
            /// Billing period start date. (inclusive)
            /// </summary>
            public DateTime StartDate { get; internal set; }
            /// <summary>
            /// Billing period end date. (exclusive)
            /// </summary>
            public DateTime EndDate { get; internal set; }
            /// <summary>
            /// Is this period already billed.
            /// </summary>
            public bool IsBilled { get; internal set; }

            public BillingPeriod() { }

            public BillingPeriod(DateTime startDate, DateTime endDate, bool isBilled)
            {
                StartDate = startDate;
                EndDate = endDate;
                IsBilled = isBilled;
            }
        }

        public class UsageInfo
        {
            public long Download { get; internal set; }

            public long Upload { get; internal set; }

            public long Total
            {
                get
                {
                    return Download + Upload;
                }
            }
        }

        public class PartialityInfo
        {
            //public decimal TariffPartiality { get; internal set; }

            public decimal GeneralPartiality { get; internal set; }
        }
        /// <summary>
        /// Subscription quota and usage details.
        /// </summary>
        public class QuotaAndUsageInfo
        {
            /// <summary>
            /// Billing period start date. (inclusive)
            /// </summary>
            public DateTime PeriodStart { get; internal set; }
            /// <summary>
            /// Billing period end date. (exclusive)
            /// </summary>
            public DateTime PeriodEnd { get; internal set; }
            /// <summary>
            /// Total quota of this period.
            /// </summary>
            public long PeriodQuota { get; internal set; }
            /// <summary>
            /// Total usage in this period.
            /// </summary>
            public long PeriodUsage { get; internal set; }
            /// <summary>
            /// Laste date which a quota added in this period.
            /// </summary>
            public DateTime LastQuotaChangeDate { get; internal set; }
            /// <summary>
            /// Last added quota amount if available.
            /// </summary>
            public long LastQuotaAmount { get; internal set; }
            /// <summary>
            /// Last quota usage.
            /// </summary>
            public long LastQuotaUsage { get; internal set; }
            /// <summary>
            /// The unused amount of quota for this period.
            /// </summary>
            public long? RemainingQuota
            {
                get
                {
                    return PeriodQuota > 0 ? Math.Max(0, PeriodQuota - PeriodUsage) : (long?)null;
                }
            }
        }

        /// <summary>
        /// Subscription usage in one day. (in bytes)
        /// </summary>
        public class DailyUsageInfo
        {
            /// <summary>
            /// Date of the day.
            /// </summary>
            public DateTime Date { get; set; }

            /// <summary>
            /// Upload amount. (in bytes)
            /// </summary>
            public long UploadBytes { get; set; }

            /// <summary>
            /// Download amount. (in bytes)
            /// </summary>
            public long DownloadBytes { get; set; }
        }
    }
}
