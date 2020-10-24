using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.ModelExtentions
{
    public static class SubscriptionExtentions
    {
        public static IQueryable<Subscription> FilterUnstableConnections(this IQueryable<Subscription> query)
        {
            return query.Where(client => client.RadiusAccountings.Where(ra => ra.StartTime > DbFunctions.AddHours(DateTime.Now, -3)).Count() > 3);
        }
    }
}
