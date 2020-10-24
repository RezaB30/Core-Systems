using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class PartnerCreditViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Amount")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [UIHint("Currency")]
        public string Amount { get; set; }

        public decimal? _amount
        {
            get
            {
                decimal parsed;
                if (decimal.TryParse(Amount, out parsed))
                    return parsed;
                return null;
            }
            set
            {
                if (value.HasValue)
                    Amount = value.Value.ToString("###,##0.00");
                else
                    Amount = null;
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Date")]
        [UIHint("ExactTime")]
        public DateTime Date { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillPayment")]
        public long? BillID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Details")]
        public string Details { get; set; }
    }
}
