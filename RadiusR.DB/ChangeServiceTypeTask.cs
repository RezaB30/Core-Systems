//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RadiusR.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChangeServiceTypeTask
    {
        public long ID { get; set; }
        public long SubscriptionID { get; set; }
        public int NewServiceID { get; set; }
        public long SchedulerTaskID { get; set; }
        public short NewBillingPeriod { get; set; }
    
        public virtual SchedulerTask SchedulerTask { get; set; }
        public virtual Service Service { get; set; }
        public virtual Subscription Subscription { get; set; }
    }
}
