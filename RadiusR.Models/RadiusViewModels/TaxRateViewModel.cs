using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class TaxRateViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Code")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [EnumType(typeof(TaxTypeID),typeof(RadiusR.Localization.Lists.TaxTypeID))]
        [UIHint("LocalizedList")]
        public short ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Rate")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Percentage(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Percentage")]
        [UIHint("Percent")]
        public string Rate
        {
            get
            {
                return (_rate * 100).ToString("0.00");
            }
            set
            {
                decimal parsed;
                if (decimal.TryParse(value, out parsed))
                {
                    _rate = Math.Round(parsed / 100m, 4);
                }
            }
        }

        public decimal _rate { get; set; }
    }
}
