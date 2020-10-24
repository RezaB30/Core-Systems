using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.QueryExtentions
{
    public static class SpecialOffersQuery
    {
        public static IQueryable<SpecialOffer> FilterActiveSpecialOffers(this IQueryable<SpecialOffer> query)
        {
            var today = DateTime.Today;
            return query.Where(so => so.IsReferral && so.StartDate <= today && so.EndDate >= today);
        }
    }
}
