using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class FeeTypeVariantViewModel
    {
        public int? ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Title")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string Title { get; set; }

        public short FeeTypeID { get; set; }


        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Price")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [UIHint("Currency")]
        public string Price
        {
            get
            {
                return _price.ToString("###,##0.00");
            }
            set
            {
                _price = decimal.Parse(value);
            }
        }

        public decimal _price { get; set; }

        public AdditionalFeeViewModel FeeType { get; set; }
    }
}