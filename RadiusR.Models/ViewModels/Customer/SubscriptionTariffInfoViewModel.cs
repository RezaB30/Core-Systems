using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.Customer
{
    public class SubscriptionTariffInfoViewModel
    {
        public int TariffID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TariffName")]
        public string TariffName { get; set; }

        public int? InQueueTariffID { get; set; }

        //public string InQueueTariffName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DomainName")]
        public string DomainName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RegistrationDate")]
        public DateTime RegistrationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ActivationDate")]
        public DateTime? ActivationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ExpirationDate")]
        public DateTime? ExpirationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CancellationDate")]
        public DateTime? CancellationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillingPeriod")]
        public int BillingPeriod { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DaysRemaining")]
        public string DaysRemaining { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "StaticIP")]
        public string StaticIP { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RemainingQuota")]
        [UIHint("FormattedBytes")]
        public decimal? RemainingQuota { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ReactivationDate")]
        public DateTime? ReactivationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "AutoPaymentType")]
        public string RecurringPaymentType { get; set; }

        public bool CanHaveQuotaSale { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CurrentBillingPeriod")]
        public DateTime? CurrentBillingPeriodStartDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CurrentBillingPeriod")]
        public DateTime? CurrentBillingPeriodEndDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LastTariffChangeDate")]
        [UIHint("ExactTime")]
        public DateTime? LastTariffChangeDate { get; set; }

        public SubscriptionScheduledTariffChangeViewModel TariffChange { get; set; }

        public bool HasBilling { get; set; }
    }
}
