﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RadiusREntities : DbContext
    {
        public RadiusREntities()
            : base("name=RadiusREntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AppSetting> AppSettings { get; set; }
        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<BTKIPBlock> BTKIPBlocks { get; set; }
        public virtual DbSet<BTKSchedulerSetting> BTKSchedulerSettings { get; set; }
        public virtual DbSet<Cashier> Cashiers { get; set; }
        public virtual DbSet<CashierBalance> CashierBalances { get; set; }
        public virtual DbSet<ChangeServiceTypeTask> ChangeServiceTypeTasks { get; set; }
        public virtual DbSet<ChangeStateTask> ChangeStateTasks { get; set; }
        public virtual DbSet<CustomerBlackList> CustomerBlackLists { get; set; }
        public virtual DbSet<CustomerSetupStatusUpdate> CustomerSetupStatusUpdates { get; set; }
        public virtual DbSet<CustomerSetupTask> CustomerSetupTasks { get; set; }
        public virtual DbSet<CustomerSetupUser> CustomerSetupUsers { get; set; }
        public virtual DbSet<Discount> Discounts { get; set; }
        public virtual DbSet<EBillRegisteredCompany> EBillRegisteredCompanies { get; set; }
        public virtual DbSet<FeeTypeCost> FeeTypeCosts { get; set; }
        public virtual DbSet<FeeTypeVariant> FeeTypeVariants { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<NASNetmap> NASNetmaps { get; set; }
        public virtual DbSet<NASVerticalIPMap> NASVerticalIPMaps { get; set; }
        public virtual DbSet<PDFFormItemPlacement> PDFFormItemPlacements { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<QuotaPackage> QuotaPackages { get; set; }
        public virtual DbSet<RadiusAccounting> RadiusAccountings { get; set; }
        public virtual DbSet<RadiusAccountingIPInfo> RadiusAccountingIPInfoes { get; set; }
        public virtual DbSet<RadiusDailyAccounting> RadiusDailyAccountings { get; set; }
        public virtual DbSet<RadiusDefault> RadiusDefaults { get; set; }
        public virtual DbSet<RadiusRBillingService> RadiusRBillingServices { get; set; }
        public virtual DbSet<RadiusSMS> RadiusSMS { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SchedulerTask> SchedulerTasks { get; set; }
        public virtual DbSet<ServiceRateTimeTable> ServiceRateTimeTables { get; set; }
        public virtual DbSet<SMSArchive> SMSArchives { get; set; }
        public virtual DbSet<SMSSetting> SMSSettings { get; set; }
        public virtual DbSet<SubscriptionCancellation> SubscriptionCancellations { get; set; }
        public virtual DbSet<SubscriptionCommitment> SubscriptionCommitments { get; set; }
        public virtual DbSet<SubscriptionCredit> SubscriptionCredits { get; set; }
        public virtual DbSet<SubscriptionGPSCoord> SubscriptionGPSCoords { get; set; }
        public virtual DbSet<SubscriptionNote> SubscriptionNotes { get; set; }
        public virtual DbSet<SubscriptionQuota> SubscriptionQuotas { get; set; }
        public virtual DbSet<SubscriptionStateHistory> SubscriptionStateHistories { get; set; }
        public virtual DbSet<SubscriptionSupportRequest> SubscriptionSupportRequests { get; set; }
        public virtual DbSet<SystemSetting> SystemSettings { get; set; }
        public virtual DbSet<TaxRate> TaxRates { get; set; }
        public virtual DbSet<RecurringPaymentSubscription> RecurringPaymentSubscriptions { get; set; }
        public virtual DbSet<ScheduledSMS> ScheduledSMS { get; set; }
        public virtual DbSet<MobilExpressAutoPayment> MobilExpressAutoPayments { get; set; }
        public virtual DbSet<EBill> EBills { get; set; }
        public virtual DbSet<ServiceBillingPeriod> ServiceBillingPeriods { get; set; }
        public virtual DbSet<CorporateCustomerInfo> CorporateCustomerInfoes { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerAdditionalPhoneNo> CustomerAdditionalPhoneNoes { get; set; }
        public virtual DbSet<CustomerIDCard> CustomerIDCards { get; set; }
        public virtual DbSet<TelekomWorkOrderParameter> TelekomWorkOrderParameters { get; set; }
        public virtual DbSet<TelekomTariff> TelekomTariffs { get; set; }
        public virtual DbSet<SubscriptionTelekomInfo> SubscriptionTelekomInfoes { get; set; }
        public virtual DbSet<TelekomWorkOrder> TelekomWorkOrders { get; set; }
        public virtual DbSet<BillFee> BillFees { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<SubscriptionTariffChange> SubscriptionTariffChanges { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<VPOSList> VPOSLists { get; set; }
        public virtual DbSet<Fee> Fees { get; set; }
        public virtual DbSet<WorkArea> WorkAreas { get; set; }
        public virtual DbSet<PartnerGroup> PartnerGroups { get; set; }
        public virtual DbSet<PartnerPermission> PartnerPermissions { get; set; }
        public virtual DbSet<PartnerCredit> PartnerCredits { get; set; }
        public virtual DbSet<TelekomAccessCredential> TelekomAccessCredentials { get; set; }
        public virtual DbSet<Domain> Domains { get; set; }
        public virtual DbSet<PartnerBillPayment> PartnerBillPayments { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<PartnerSubUser> PartnerSubUsers { get; set; }
        public virtual DbSet<SMSText> SMSTexts { get; set; }
        public virtual DbSet<PartnerAvailableTariff> PartnerAvailableTariffs { get; set; }
        public virtual DbSet<SystemLog> SystemLogs { get; set; }
        public virtual DbSet<NASExpiredPool> NASExpiredPools { get; set; }
        public virtual DbSet<PartnerRegisteredSubscription> PartnerRegisteredSubscriptions { get; set; }
        public virtual DbSet<RecurringDiscount> RecurringDiscounts { get; set; }
        public virtual DbSet<SpecialOffer> SpecialOffers { get; set; }
        public virtual DbSet<AppliedRecurringDiscount> AppliedRecurringDiscounts { get; set; }
        public virtual DbSet<OfflinePaymentGateway> OfflinePaymentGateways { get; set; }
        public virtual DbSet<ExternalPayment> ExternalPayments { get; set; }
        public virtual DbSet<NAS> NAS { get; set; }
        public virtual DbSet<SupportGroup> SupportGroups { get; set; }
        public virtual DbSet<SupportGroupUser> SupportGroupUsers { get; set; }
        public virtual DbSet<SupportRequest> SupportRequests { get; set; }
        public virtual DbSet<SupportRequestSubType> SupportRequestSubTypes { get; set; }
        public virtual DbSet<SupportRequestType> SupportRequestTypes { get; set; }
        public virtual DbSet<SupportRequestProgress> SupportRequestProgresses { get; set; }
    }
}
