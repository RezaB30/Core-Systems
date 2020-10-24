using RadiusR.DB.Enums;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class ServiceViewModel
    {
        public int ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "ServiceName")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        //[WordAndNumber(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "WordAndNumber")]
        [MaxLength(64, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "RateLimit")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        //[RateLimit(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "RateLimit")]
        //[UIHint("RateLimit")]
        public string RateLimit
        {
            get
            {
                return RateLimitView != null ? RateLimitView.ToString() : null;
            }
            set
            {
                RateLimitView = MikrotikRateLimitViewModel.Parse(value) ?? new MikrotikRateLimitViewModel();
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "RateLimit")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [UIHint("RateLimit")]
        public MikrotikRateLimitViewModel RateLimitView { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Price")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [UIHint("Currency")]
        public string Price
        {
            get
            {
                return _price.HasValue ? _price.Value.ToString("###,##0.00") : null;
            }
            set
            {
                decimal price;
                if (decimal.TryParse(value, out price))
                    _price = price;
            }
        }

        [UIHint("Currency")]
        public decimal? _price { get; set; }

        #region QuotaProps

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "MaxSmartQuotaPrice")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [UIHint("Currency")]
        public string MaxSmartQuotaPrice
        {
            get
            {
                return _maxSmartQuotaPrice.HasValue ? _maxSmartQuotaPrice.Value.ToString("###,##0.00") : null;
            }
            set
            {
                decimal maxPrice;
                if (decimal.TryParse(value, out maxPrice))
                    _maxSmartQuotaPrice = maxPrice;
            }
        }

        [UIHint("Currency")]
        public decimal? _maxSmartQuotaPrice { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BaseQuota")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [PositiveLong(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveLong")]
        [UIHint("TrafficLimit")]
        public string BaseQuota { get; set; }

        public long? _baseQuota
        {
            get
            {
                long val = 0;
                if (long.TryParse(BaseQuota, out val))
                    return val;
                return null;
            }
            set
            {
                BaseQuota = value.ToString();
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SoftQuotaRateLimit")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        //[RateLimit(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "RateLimit")]

        public string SoftQuotaRateLimit
        {
            get
            {
                return SoftQuotaRateLimitView != null ? SoftQuotaRateLimitView.ToString() : null;
            }
            set
            {
                SoftQuotaRateLimitView = MikrotikRateLimitViewModel.Parse(value) ?? new MikrotikRateLimitViewModel();
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SoftQuotaRateLimit")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [UIHint("RateLimit")]
        public MikrotikRateLimitViewModel SoftQuotaRateLimitView { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "QuotaType")]
        [EnumType(typeof(QuotaType), typeof(RadiusR.Localization.Lists.QuotaType))]
        [UIHint("LocalizedList")]
        public short? QuotaType { get; set; }

        #endregion

        public bool IsActive { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "InfrastructureType")]
        [EnumType(typeof(ServiceInfrastructureTypes), typeof(RadiusR.Localization.Lists.ServiceInfrastuctureTypes))]
        [UIHint("LocalizedList")]
        public short InfrastructureType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillingType")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [EnumType(typeof(ServiceBillingType), typeof(RadiusR.Localization.Lists.ServiceBillingType))]
        [UIHint("LocalizedList")]
        public short BillingType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ServiceRateTimeTable")]
        [UIHint("ServiceRateTimeTable")]
        public ICollection<ServiceRateTimePartitionViewModel> ServiceRateTimeTable { get; set; }

        public bool CanHaveQuotaSale { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ServiceDomains")]
        public IEnumerable<ServiceDomainViewModel> ServiceDomains { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ServiceDomains")]
        //[Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int[] DomainIDs { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillingPeriods")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public IEnumerable<short> BillingPeriods { get; set; }


        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "NoQueue")]
        public bool NoQueue { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "PaymentTolerance")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        [MaxLength(2, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string PaymentTolerance { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "ExpirationTolerance")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        [MaxLength(2, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string ExpirationTolerance { get; set; }

        public int? _paymentTolerance
        {
            get
            {
                int parsed;
                if (int.TryParse(PaymentTolerance, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                PaymentTolerance = value.ToString();
            }
        }

        public int? _expirationTolerance
        {
            get
            {
                int parsed;
                if (int.TryParse(ExpirationTolerance, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                ExpirationTolerance = value.ToString();
            }
        }

        public bool HasConflictingTimeTable()
        {
            if (ServiceRateTimeTable == null)
                return false;
            var items = ServiceRateTimeTable.ToArray();
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i]._startTime == items[i]._endTime)
                    return true;
                for (int j = i + 1; j < items.Length; j++)
                {
                    if (items[i]._startTime < items[i]._endTime)
                    {
                        if (items[j]._startTime < items[i]._endTime && items[j]._endTime > items[i]._startTime)
                            return true;
                    }
                    else
                    {
                        if (items[j]._startTime > items[j]._endTime || items[j]._startTime < items[i]._endTime || items[j]._endTime > items[i]._startTime)
                            return true;
                    }
                }
            }

            return false;
        }
    }
}