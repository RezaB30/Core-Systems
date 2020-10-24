using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.QueryExtentions
{
    public static class ServicesQuery
    {
        public static IQueryable<Service> FilterActiveServices(this IQueryable<Service> query, int? currentId = null)
        {
            return query.Where(service => service.IsActive || service.ID == currentId);
        }
    }
}
