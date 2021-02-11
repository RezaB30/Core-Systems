using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public partial class PartnerRegisteredSubscription
    {
        // NOTICE: HAS RELATIVE PART IN QUERY EXTENTIONS. THESE SHOULD ONLY BE CHANGED SIMULTANIOUSLY!!!
        public bool HasFullBills
        {
            get
            {
                return Subscription.Bills.Any(b => b.Source == (short)Enums.BillSources.System && b.PeriodEnd.HasValue && b.PeriodStart.HasValue && b.PeriodStart.Value.AddMonths(1) <= b.PeriodEnd);
            }
        }
    }
}
