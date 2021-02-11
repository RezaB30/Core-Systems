using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.QueryExtentions
{
    public static class PartnerRegisteredSubscriptionQuery
    {
        // NOTICE: HAS RELATIVE PART IN MODEL EXTENTIONS. THESE SHOULD ONLY BE CHANGED SIMULTANIOUSLY!!!
        public static IQueryable<PartnerRegisteredSubscription> FilterSubscriptionsWithAtLeastOneFullBill(this IQueryable<PartnerRegisteredSubscription> query)
        {
            return query.Where(prs => prs.Subscription.Bills.Any(b => b.Source == (short)Enums.BillSources.System && (DbFunctions.DiffMonths(b.PeriodStart, b.PeriodEnd) == 1 && b.PeriodStart.Value.Day <= b.PeriodEnd.Value.Day) || (DbFunctions.DiffMonths(b.PeriodStart, b.PeriodEnd) > 1)));
        }
    }
}
