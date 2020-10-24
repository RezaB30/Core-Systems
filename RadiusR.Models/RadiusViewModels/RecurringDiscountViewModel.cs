using RadiusR.DB.Enums;
using RadiusR.DB.Enums.RecurringDiscount;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class RecurringDiscountViewModel
    {
        public long ID { get; set; }

        //public long SubscriptionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CreationDate")]
        [UIHint("ExactTime")]
        public DateTime CreationTime { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DiscountType")]
        [EnumType(typeof(RecurringDiscountType), typeof(RadiusR.Localization.Lists.RecurringDiscount.RecurringDiscountType))]
        [UIHint("LocalizedList")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public short DiscountType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ApplicationType")]
        [EnumType(typeof(RecurringDiscountApplicationType), typeof(RadiusR.Localization.Lists.RecurringDiscount.RecurringDiscountApplicationType))]
        [UIHint("LocalizedList")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public short ApplicationType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Amount")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [UIHint("Currency")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string Amount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Amount")]
        [Percentage(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Percentage")]
        [UIHint("Percent")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string PercentageAmount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ApplicationTimes")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public int? ApplicationTimes { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FeeTypeID")]
        [EnumType(typeof(FeeType), typeof(RadiusR.Localization.Lists.FeeType))]
        [UIHint("LocalizedList")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public short? FeeTypeID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "OnlyFullInvoice")]
        public bool OnlyFullInvoice { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Description")]
        [MaxLength(300, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TimesApplied")]
        public int TimesApplied { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Referrer")]
        public string ReferrerCode { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Referring")]
        public string ReferringCode { get; set; }

        public long? ReferencedSubscriptionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CancellationDate")]
        public DateTime? CancellationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CancellationReason")]
        [EnumType(typeof(RecurringDiscountCancellationCause), typeof(RadiusR.Localization.Lists.RecurringDiscount.RecurringDiscountCancellationCause))]
        [UIHint("LocalizedList")]
        public short? CancellationCause { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsFinished
        {
            get
            {
                return TimesApplied >= ApplicationTimes;
            }
        }

        public decimal? _amount
        {
            get
            {
                if (Enum.IsDefined(typeof(RecurringDiscountType), (int)DiscountType))
                {
                    decimal value;
                    var currentDiscountType = (RecurringDiscountType)DiscountType;
                    switch (currentDiscountType)
                    {
                        case RecurringDiscountType.Static:
                            if (decimal.TryParse(Amount, out value))
                                return value;
                            break;
                        case RecurringDiscountType.Percentage:
                            if (decimal.TryParse(PercentageAmount, out value))
                                return Math.Round(value / 100m, 4);
                            break;
                        default:
                            break;
                    }
                }
                return null;
            }
            set
            {
                Amount = value.HasValue ? value.Value.ToString("###,###.00") : null;
                PercentageAmount = value.HasValue ? (value.Value * 100).ToString("#0.00") : null;
            }
        }
    }
}
