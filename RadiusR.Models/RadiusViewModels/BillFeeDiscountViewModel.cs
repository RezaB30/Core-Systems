using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class BillFeeDiscountViewModel
    {
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public long BillFeeID { get; set; }

        public string FeeTitle { get; set; }

        [UIHint("Currency")]
        public string FeeCost { get; set; }

        public decimal? _feeCost
        {
            get
            {
                decimal parsed;
                if (decimal.TryParse(FeeCost, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                FeeCost = value.HasValue ? value.Value.ToString("###,###,##0.00") : null;
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Discount")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [UIHint("Currency")]
        public string DiscountAmount { get; set; }

        public decimal? _discountAmount
        {
            get
            {
                decimal parsed;
                if (decimal.TryParse(DiscountAmount, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                DiscountAmount = value.HasValue ? value.Value.ToString("###,###,##0.00") : null;
            }
        }
    }
}
