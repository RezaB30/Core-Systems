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
    
    public partial class Subscription
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Subscription()
        {
            this.Bills = new HashSet<Bill>();
            this.ChangeServiceTypeTasks = new HashSet<ChangeServiceTypeTask>();
            this.ChangeStateTasks = new HashSet<ChangeStateTask>();
            this.Fees = new HashSet<Fee>();
            this.RadiusAccountings = new HashSet<RadiusAccounting>();
            this.RadiusDailyAccountings = new HashSet<RadiusDailyAccounting>();
            this.RadiusSMS = new HashSet<RadiusSMS>();
            this.RecurringDiscounts = new HashSet<RecurringDiscount>();
            this.ScheduledSMSes = new HashSet<ScheduledSMS>();
            this.SMSArchives = new HashSet<SMSArchive>();
            this.SubscriptionCredits = new HashSet<SubscriptionCredit>();
            this.SubscriptionNotes = new HashSet<SubscriptionNote>();
            this.SubscriptionQuotas = new HashSet<SubscriptionQuota>();
            this.SubscriptionStateHistories = new HashSet<SubscriptionStateHistory>();
            this.SubscriptionSupportRequests = new HashSet<SubscriptionSupportRequest>();
            this.SubscriptionTariffHistories = new HashSet<SubscriptionTariffHistory>();
            this.SystemLogs = new HashSet<SystemLog>();
            this.SupportRequests = new HashSet<SupportRequest>();
            this.CustomerSetupTasks = new HashSet<CustomerSetupTask>();
            this.Groups = new HashSet<Group>();
            this.SubscriptionTransferredFromHistories = new HashSet<SubscriptionTransferHistory>();
            this.SubscriptionTransferredToHistories = new HashSet<SubscriptionTransferHistory>();
            this.TelekomWorkOrders = new HashSet<TelekomWorkOrder>();
            this.BTKSubscriptionChanges = new HashSet<BTKSubscriptionChange>();
        }
    
        public long ID { get; set; }
        public long CustomerID { get; set; }
        public long AddressID { get; set; }
        public System.DateTime MembershipDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public short State { get; set; }
        public int PaymentDay { get; set; }
        public int ServiceID { get; set; }
        public Nullable<System.DateTime> ActivationDate { get; set; }
        public string OnlinePassword { get; set; }
        public string SubscriberNo { get; set; }
        public Nullable<System.DateTime> OnlinePasswordExpirationDate { get; set; }
        public bool ArchiveScanned { get; set; }
        public int DomainID { get; set; }
        public string ReferenceNo { get; set; }
        public Nullable<System.DateTime> LastTariffChangeDate { get; set; }
        public short RegistrationType { get; set; }
    
        public virtual Address Address { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bill> Bills { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChangeServiceTypeTask> ChangeServiceTypeTasks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChangeStateTask> ChangeStateTasks { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Domain Domain { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Fee> Fees { get; set; }
        public virtual MobilExpressAutoPayment MobilExpressAutoPayment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RadiusAccounting> RadiusAccountings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RadiusDailyAccounting> RadiusDailyAccountings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RadiusSMS> RadiusSMS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RecurringDiscount> RecurringDiscounts { get; set; }
        public virtual RecurringPaymentSubscription RecurringPaymentSubscription { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScheduledSMS> ScheduledSMSes { get; set; }
        public virtual Service Service { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SMSArchive> SMSArchives { get; set; }
        public virtual SubscriptionCancellation SubscriptionCancellation { get; set; }
        public virtual SubscriptionCommitment SubscriptionCommitment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubscriptionCredit> SubscriptionCredits { get; set; }
        public virtual SubscriptionGPSCoord SubscriptionGPSCoord { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubscriptionNote> SubscriptionNotes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubscriptionQuota> SubscriptionQuotas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubscriptionStateHistory> SubscriptionStateHistories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubscriptionSupportRequest> SubscriptionSupportRequests { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubscriptionTariffHistory> SubscriptionTariffHistories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SystemLog> SystemLogs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SupportRequest> SupportRequests { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerSetupTask> CustomerSetupTasks { get; set; }
        public virtual PartnerRegisteredSubscription PartnerRegisteredSubscription { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Group> Groups { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubscriptionTransferHistory> SubscriptionTransferredFromHistories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubscriptionTransferHistory> SubscriptionTransferredToHistories { get; set; }
        public virtual SubscriptionTelekomInfo SubscriptionTelekomInfo { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TelekomWorkOrder> TelekomWorkOrders { get; set; }
        public virtual RadiusAuthorization RadiusAuthorization { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BTKSubscriptionChange> BTKSubscriptionChanges { get; set; }
    }
}
