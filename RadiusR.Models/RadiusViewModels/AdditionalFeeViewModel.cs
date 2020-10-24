using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class AdditionalFeeViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FeeTypeID")]
        [EnumType(typeof(FeeType), typeof(RadiusR.Localization.Lists.FeeType))]
        [UIHint("LocalizedList")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public short FeeTypeID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Price")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [UIHint("Currency")]
        public string Price
        {
            get
            {
                return (_price.HasValue) ? _price.Value.ToString("###,##0.00") : null;
            }
            set
            {
                _price = (string.IsNullOrEmpty(value)) ? (decimal?)null : decimal.Parse(value);
            }
        }

        public decimal? _price { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaxTypes")]
        public IEnumerable<TaxRateViewModel> TaxTypes { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IsAllTime")]
        public bool IsAllTime { get; set; }

        public bool HasVariants { get; set; }

        public IEnumerable<FeeTypeVariantViewModel> FeeTypeVariants { get; set; }
    }
}