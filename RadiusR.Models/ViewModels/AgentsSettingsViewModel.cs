using RezaB.DBUtilities;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class AgentsSettingsViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "NonCashPaymentCommission")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Percentage(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Percentage")]
        [UIHint("Percent")]
        public string AgentsNonCashPaymentCommissionDisplay { get; set; }

        [SettingElement]
        public decimal? AgentsNonCashPaymentCommission
        {
            get
            {
                decimal value;
                if (decimal.TryParse(AgentsNonCashPaymentCommissionDisplay, out value))
                {
                    return value / 100m;
                }
                return null;
            }
            set
            {
                AgentsNonCashPaymentCommissionDisplay = (value * 100m)?.ToString("#0.00");
            }
        }
    }
}
