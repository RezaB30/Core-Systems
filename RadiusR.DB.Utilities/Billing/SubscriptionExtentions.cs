using RadiusR.DB.Enums;
using RadiusR.DB.ModelExtentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Billing
{
    public static class SubscriptionExtentions
    {
        public static void UpdateLastAllowedDate(this Subscription dbSubscription, bool forceChange = false, Bill newlyAddedBill = null)
        {
            if (!dbSubscription.IsActive || !dbSubscription.ActivationDate.HasValue || dbSubscription.Service.BillingType == (short)ServiceBillingType.PrePaid)
                return;
            var currentValue = dbSubscription.LastAllowedDate;
            var queryBase = dbSubscription.Bills.Concat(newlyAddedBill != null ? new[] { newlyAddedBill } : Enumerable.Empty<Bill>());
            var firstUnpaidBill = queryBase.Where(b => b.Source == (short)BillSources.System && b.BillStatusID == (short)BillState.Unpaid).OrderBy(b => b.DueDate).FirstOrDefault();
            var billsQuery = queryBase.OrderByDescending(b => b.DueDate).Where(b => b.Source == (short)BillSources.System && b.BillStatusID != (short)BillState.Unpaid);
            if (firstUnpaidBill != null)
                billsQuery = billsQuery.Where(b => b.DueDate < firstUnpaidBill.DueDate);

            var lastContinuousPaidBill = billsQuery.FirstOrDefault();
            var periodCheckDate = (lastContinuousPaidBill == null || lastContinuousPaidBill.PeriodEnd < dbSubscription.ActivationDate) ? dbSubscription.ActivationDate : lastContinuousPaidBill.PeriodEnd;
            var lastUnpaidBillingPeriod = dbSubscription.GetCurrentBillingPeriod(periodCheckDate, true);
            var newLastAllowedDate = (dbSubscription.Service.BillingType == (short)ServiceBillingType.Invoiced) ? lastUnpaidBillingPeriod.EndDate.AddDays(dbSubscription.Service.PaymentTolerance + dbSubscription.Service.ExpirationTolerance) : lastUnpaidBillingPeriod.StartDate.AddDays(dbSubscription.Service.PaymentTolerance + dbSubscription.Service.ExpirationTolerance);

            if (newLastAllowedDate <= dbSubscription.LastAllowedDate && !forceChange)
                return;

            dbSubscription.LastAllowedDate = newLastAllowedDate;
        }
    }
}
