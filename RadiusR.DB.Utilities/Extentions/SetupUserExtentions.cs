using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Extentions
{
    public static class SetupUserExtentions
    {
        public static IQueryable<CustomerSetupUser> ActiveUsers(this IQueryable<CustomerSetupUser> query)
        {
            return query.Where(user => user.IsEnabled);
        }

        public static IQueryable<CustomerSetupUser> ValidPartnersForArea(this IQueryable<CustomerSetupUser> query, Address setupAddress)
        {
            return query.Where(su => !su.Partners.Any() ||
                (su.Partners.FirstOrDefault().WorkAreas.Any(wa => wa.ProvinceID == setupAddress.ProvinceID && !wa.DistrictID.HasValue) ||
                su.Partners.FirstOrDefault().WorkAreas.Any(wa => wa.DistrictID == setupAddress.DistrictID && !wa.RuralCode.HasValue) ||
                su.Partners.FirstOrDefault().WorkAreas.Any(wa => wa.RuralCode == setupAddress.RuralCode && !wa.NeighbourhoodID.HasValue) ||
                su.Partners.FirstOrDefault().WorkAreas.Any(wa => wa.NeighbourhoodID == setupAddress.NeighborhoodID)));
        }

        public static IQueryable<CustomerSetupUser> ValidAgents(this IQueryable<CustomerSetupUser> query, int? agentId)
        {
            return query.Where(su => !su.Agents.Any() || su.Agents.Any(a => a.ID == agentId));
        }
    }
}
