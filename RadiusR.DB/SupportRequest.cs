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
    
    public partial class SupportRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SupportRequest()
        {
            this.SupportRequestProgresses = new HashSet<SupportRequestProgress>();
        }
    
        public long ID { get; set; }
        public string SupportPin { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<long> SubscriptionID { get; set; }
        public short StateID { get; set; }
        public Nullable<int> TypeID { get; set; }
        public Nullable<int> SubTypeID { get; set; }
        public bool IsVisibleToCustomer { get; set; }
        public Nullable<int> AssignedGroupID { get; set; }
        public Nullable<int> AssignedUserID { get; set; }
        public Nullable<System.DateTime> CustomerApprovalDate { get; set; }
        public Nullable<int> RedirectedGroupID { get; set; }
    
        public virtual AppUser AssignedUser { get; set; }
        public virtual Subscription Subscription { get; set; }
        public virtual SupportGroup RedirectedSupportGroup { get; set; }
        public virtual SupportGroup AssignedSupportGroup { get; set; }
        public virtual SupportRequestSubType SupportRequestSubType { get; set; }
        public virtual SupportRequestType SupportRequestType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SupportRequestProgress> SupportRequestProgresses { get; set; }
    }
}
