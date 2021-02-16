using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.QueryExtentions
{
    public static class GroupsQuery
    {
        public static IQueryable<Group> GetValidGroupsForSubscriptions(this IQueryable<Group> query)
        {
            return query.Where(g => g.IsActive);
        }
    }
}
