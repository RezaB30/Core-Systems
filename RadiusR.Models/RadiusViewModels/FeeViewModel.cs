using RadiusR.DB;
using RadiusR.DB.Utilities.Billing;
using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RezaB.Web.CustomAttributes;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class FeeViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FeeTypeID")]
        [EnumType(typeof(FeeType), typeof(RadiusR.Localization.Lists.FeeType))]
        [UIHint("LocalizedList")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public short FeeTypeID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "InstallmentCount")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public short InstallmentBillCount { get; set; }

        public long SubscriptionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        public DateTime Date { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Description")]
        public string Description { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Price")]
        [UIHint("Currency")]
        public decimal? Cost { get; set; }

        public bool CanBeCancelled { get; set; }

        public bool IsCancelled { get; set; }

        public IEnumerable<BillFeeViewModel> BillFees { get; set; }

        public AdditionalFeeViewModel FeeType { get; set; }

        public FeeTypeVariantViewModel FeeTypeVariant { get; set; }
    }
}