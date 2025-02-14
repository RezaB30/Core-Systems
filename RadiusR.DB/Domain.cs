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
    
    public partial class Domain
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Domain()
        {
            this.Services = new HashSet<Service>();
            this.Subscriptions = new HashSet<Subscription>();
            this.ExternalTariffs = new HashSet<ExternalTariff>();
            this.PartnerAvailableTariffs = new HashSet<PartnerAvailableTariff>();
            this.AgentTariffs = new HashSet<AgentTariff>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string UsernamePrefix { get; set; }
        public string SubscriberNoPrefix { get; set; }
        public Nullable<short> AccessMethod { get; set; }
        public int MaxFreezeDuration { get; set; }
        public short MaxFreezesPerYear { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Service> Services { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Subscription> Subscriptions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExternalTariff> ExternalTariffs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartnerAvailableTariff> PartnerAvailableTariffs { get; set; }
        public virtual TelekomAccessCredential TelekomAccessCredential { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AgentTariff> AgentTariffs { get; set; }
    }
}
