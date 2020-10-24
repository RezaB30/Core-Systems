using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class QuotaPackageViewModel
    {
        public int ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Name")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Amount")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [PositiveLong(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveLong")]
        [UIHint("TrafficLimit")]
        public string Amount { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Price")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [UIHint("Currency")]
        public string Price { get; set; }

        public decimal? _price
        {
            get
            {
                decimal parsed;
                if (decimal.TryParse(Price, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                Price = value.HasValue ? value.Value.ToString("###,##0.00") : null;
            }
        }

        public long? _amount
        {
            get
            {
                long parsed;
                if (long.TryParse(Amount, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                Amount = value.HasValue ? value.Value.ToString() : null;
            }
        }
    }
}
