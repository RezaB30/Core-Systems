using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.TelekomOperations.Wrappers
{
    public static class WorkOrderQueryExtentions
    {
        public static IQueryable<QueryReadyWorkOrder> PrepareForStatusCheck(this IQueryable<TelekomWorkOrder> query)
        {
            return query.Select(two => new QueryReadyWorkOrder()
            {
                ID = two.ID,
                OperationType = (Enums.TelekomOperations.TelekomOperationType)two.OperationTypeID,
                DomainID = two.Subscription.DomainID,
                TelekomCustomerCode = two.Subscription.SubscriptionTelekomInfo != null ? two.Subscription.SubscriptionTelekomInfo.TTCustomerCode : (long?)null,
                QueueNo = two.QueueNo,
                ManagementCode = two.ManagementCode,
                ProvinceCode = two.ProvinceCode,
                TransactionID = two.TransactionID,
                XDSLNo = two.Subscription.SubscriptionTelekomInfo != null ? two.Subscription.SubscriptionTelekomInfo.SubscriptionNo : null
            });
        }
    }
}
