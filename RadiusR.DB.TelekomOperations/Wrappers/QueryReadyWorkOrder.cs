using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.TelekomOperations.Wrappers
{
    public class QueryReadyWorkOrder
    {
        public long ID { get; internal set; }

        public Enums.TelekomOperations.TelekomOperationType OperationType { get; internal set; }

        public int DomainID { get; internal set; }

        public long? TelekomCustomerCode { get; internal set; }

        public long? QueueNo { get; internal set; }

        public int? ManagementCode { get; internal set; }

        public int? ProvinceCode { get; internal set; }

        public long? TransactionID { get; internal set; }

        public string XDSLNo { get; set; }

        internal QueryReadyWorkOrder() { }

        public QueryReadyWorkOrder(TelekomWorkOrder dbWorkOrder)
        {
            ID = dbWorkOrder.ID;
            OperationType = (Enums.TelekomOperations.TelekomOperationType)dbWorkOrder.OperationTypeID;
            DomainID = dbWorkOrder.Subscription.DomainID;
            TelekomCustomerCode = dbWorkOrder.Subscription.SubscriptionTelekomInfo != null ? dbWorkOrder.Subscription.SubscriptionTelekomInfo.TTCustomerCode : (long?)null;
            QueueNo = dbWorkOrder.QueueNo;
            ManagementCode = dbWorkOrder.ManagementCode;
            ProvinceCode = dbWorkOrder.ProvinceCode;
            TransactionID = dbWorkOrder.TransactionID;
            XDSLNo = dbWorkOrder.Subscription.SubscriptionTelekomInfo?.SubscriptionNo;
        }
    }
}
