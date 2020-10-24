using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace RadiusR.SMS
{
    public static class DBExtentions
    {
        public static IQueryable<Subscription> PrepareForSMS(this IQueryable<Subscription> query)
        {
            return query
                .Include(s => s.Address)
                .Include(s => s.Customer.Address)
                .Include(s => s.Customer.CorporateCustomerInfo)
                .Include(s => s.Service);
        }

        public static IQueryable<Bill> PrepareForSMS(this IQueryable<Bill> query)
        {
            return query
                .Include(b => b.Subscription.Address)
                .Include(b => b.Subscription.Customer.Address)
                .Include(b => b.Subscription.Customer.CorporateCustomerInfo)
                .Include(b => b.Subscription.Service);
        }

        public static IQueryable<ScheduledSMS> PrepareForSMS(this IQueryable<ScheduledSMS> query)
        {
            return query
                .Include(ss => ss.Subscription.Address)
                .Include(ss => ss.Subscription.Customer.Address)
                .Include(ss => ss.Subscription.Customer.CorporateCustomerInfo)
                .Include(ss => ss.Subscription.Service);
        }
    }
}
