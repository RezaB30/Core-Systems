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
    
    public partial class Agent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Agent()
        {
            this.AgentRelatedPayments = new HashSet<AgentRelatedPayment>();
            this.Subscriptions = new HashSet<Subscription>();
            this.WorkAreas = new HashSet<WorkArea>();
            this.AgentCollections = new HashSet<AgentCollection>();
        }
    
        public int ID { get; set; }
        public string ExecutiveName { get; set; }
        public string CompanyTitle { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNo { get; set; }
        public long AddressID { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public decimal Allowance { get; set; }
        public string Password { get; set; }
        public bool IsEnanbled { get; set; }
        public int CustomerSetupUserID { get; set; }
    
        public virtual Address Address { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AgentRelatedPayment> AgentRelatedPayments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Subscription> Subscriptions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkArea> WorkAreas { get; set; }
        public virtual CustomerSetupUser CustomerSetupUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AgentCollection> AgentCollections { get; set; }
    }
}
