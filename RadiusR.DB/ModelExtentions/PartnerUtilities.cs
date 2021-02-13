using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.ModelExtentions
{
    public static class PartnerUtilities
    {
        public static Dictionary<Enums.PartnerAllowanceState, decimal> GetAllowanceDetails(this RadiusREntities db, int partnerId, Enums.PartnerCollectionType collectionType)
        {
            switch (collectionType)
            {
                case Enums.PartnerCollectionType.Setup:
                    {
                        var dbPartner = db.Partners.Find(partnerId);
                        if (dbPartner.CustomerSetupUserID.HasValue)
                            return db.CustomerSetupTasks.Where(cst => cst.SetupUserID == dbPartner.CustomerSetupUserID).GroupBy(cst => cst.AllowanceState).Select(g => new { Key = g.Key, Value = g.Select(cst => cst.Allowance ?? 0m).DefaultIfEmpty(0m).Sum() }).ToDictionary(item => (Enums.PartnerAllowanceState)item.Key, item => item.Value);
                        else
                            return new Dictionary<Enums.PartnerAllowanceState, decimal>();
                    }
                case Enums.PartnerCollectionType.Sales:
                    return db.PartnerRegisteredSubscriptions.Where(prs => prs.PartnerID == partnerId).GroupBy(prs => prs.AllowanceState).Select(g=> new { Key = g.Key, Value = g.Select(prs => prs.Allowance).DefaultIfEmpty(0m).Sum() }).ToDictionary(item => (Enums.PartnerAllowanceState)item.Key, item => item.Value);
                default:
                    return null;
            }
        }
    }
}
