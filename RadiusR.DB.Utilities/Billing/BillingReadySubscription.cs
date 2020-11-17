using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Billing
{
    public class BillingReadySubscription
    {
        public Subscription Subscription { get; set; }

        public IEnumerable<BillingReadyRecurringDiscount> ValidRecurringDiscounts { get; set; }

        public decimal CreditsTotal { get; set; }

        public IEnumerable<BillingReadySubscriptionFee> ValidFees { get; set; }

        public IEnumerable<BillingReadySubscriptionUsage> Usage { get; set; }
    }

    public class BillingReadyRecurringDiscount
    {
        public RecurringDiscount RecurringDiscount { get; set; }

        public int AppliedTimes { get; set; }

        public bool HasBeenPenalized { get; set; }

        public int ApplicationTimes { get; set; }

        public long? ReferralSubscriptionID { get; set; }

        public bool OnlyFullInvoice { get; set; }

        public IEnumerable<ReferralSubscriptionBillInfo> PairedSubscriptionBills { get; set; }
    }

    public class ReferralSubscriptionBillInfo
    {
        public long ID { get; set; }

        public DateTime IssueDate { get; set; }

        public short State { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? PayDate { get; set; }
    }

    public class BillingReadySubscriptionFee
    {
        public bool IsAllTime { get; set; }

        public short FeeTypeID { get; set; }

        public decimal Cost { get; set; }

        public long ID { get; set; }

        public int CountedInstallments { get; set; }

        public int TotalInstallments { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }

    public class BillingReadySubscriptionUsage
    {
        public DateTime Date { get; set; }

        public long DownloadBytes { get; set; }

        public long UploadBytes { get; set; }
    }

    public static class BillingReadyQuerySelection
    {
        public static IQueryable<BillingReadySubscription> PrepareForBilling(this RadiusREntities db, IQueryable<Subscription> query)
        {
            return query
                .Include(s => s.Bills)
                .Include(s => s.Service)
                //.Include(s => s.SubscriptionTariffChange.Service)
                .Select(subscription => new BillingReadySubscription()
                {
                    Subscription = subscription,
                    ValidRecurringDiscounts = subscription.RecurringDiscounts.Where(rd => !rd.IsDisabled).Select(rd => new
                    {
                        RecurringDiscount = rd,
                        AppliedTimes = rd.AppliedRecurringDiscounts.Count(),
                        ApplicationTimes = rd.ApplicationTimes,
                        OnlyFullInvoice = rd.OnlyFullInvoice,
                        ReferralSubscriptionID = (rd.ReferrerRecurringDiscount != null || rd.ReferringRecurringDiscounts.Any()) ? (rd.ReferrerRecurringDiscount ?? rd.ReferringRecurringDiscounts.FirstOrDefault()).SubscriptionID : (long?)null,
                    }).Select(rd => new BillingReadyRecurringDiscount()
                    {
                        RecurringDiscount = rd.RecurringDiscount,
                        AppliedTimes = rd.AppliedTimes,
                        ApplicationTimes = rd.ApplicationTimes,
                        ReferralSubscriptionID = rd.ReferralSubscriptionID,
                        OnlyFullInvoice = rd.OnlyFullInvoice,
                        PairedSubscriptionBills = db.Bills.Where(b => b.SubscriptionID == rd.ReferralSubscriptionID.Value).Where(b => b.BillStatusID != (short)BillState.Cancelled && b.Source == (short)BillSources.System).OrderByDescending(b => b.IssueDate).ThenByDescending(b => b.ID).Select(b => new ReferralSubscriptionBillInfo()
                        {
                            ID = b.ID,
                            IssueDate = b.IssueDate,
                            State = b.BillStatusID,
                            DueDate = b.DueDate,
                            PayDate = DbFunctions.TruncateTime(b.PayDate)
                        }).Select(b => b)
                    }).Where(rd => rd.AppliedTimes < rd.RecurringDiscount.ApplicationTimes).Select(rd => rd),
                    CreditsTotal = subscription.SubscriptionCredits.Select(sc => sc.Amount).DefaultIfEmpty(0m).Sum(),
                    ValidFees = subscription.Fees.Where(f => !f.IsCancelled).Select(f => new BillingReadySubscriptionFee()
                    {
                        IsAllTime = f.FeeTypeCost.IsAllTime,
                        FeeTypeID = f.FeeTypeID,
                        Cost = f.Cost ?? f.FeeTypeCost.Cost ?? f.FeeTypeVariant.Price,
                        ID = f.ID,
                        CountedInstallments = f.BillFees.Where(bf => bf.Bill.BillStatusID != (short)BillState.Cancelled).Select(bf => bf.InstallmentCount).DefaultIfEmpty(0).Sum(),
                        TotalInstallments = f.InstallmentBillCount,
                        StartDate = f.StartDate,
                        EndDate = f.EndDate
                    }).Where(f => f.IsAllTime || f.CountedInstallments < f.TotalInstallments).Select(f => f),
                    Usage = subscription.RadiusDailyAccountings.Where(rda => rda.Date >= subscription.Bills.Where(b => b.Source == (short)BillSources.System).Select(b => b.PeriodEnd).OrderByDescending(pe => pe).DefaultIfEmpty(subscription.ActivationDate).Max()).Where(rda => rda.Subscription.Service.QuotaType == (short)QuotaType.SmartQuota)
                    .Select(rda => new BillingReadySubscriptionUsage()
                    {
                        Date = rda.Date,
                        DownloadBytes = rda.DownloadBytes,
                        UploadBytes = rda.UploadBytes
                    })
                });
        }
    }
}
