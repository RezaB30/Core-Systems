using RadiusR.DB.Settings;
using RezaB.DBUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Web.CustomAttributes;

namespace RadiusR_Manager.Models.ViewModels
{
    public class QuotaSettingsViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "QuotaUnit")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [PositiveLong(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveLong")]
        [UIHint("TrafficLimit")]
        public string QuotaUnitDisplay { get; set; }

        [SettingElement]
        public long? QuotaUnit
        {
            get
            {
                long val = 0;
                if (long.TryParse(QuotaUnitDisplay, out val))
                    return val;
                return null;
            }
            set
            {
                QuotaUnitDisplay = value.ToString();
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "QuotaUnitPrice")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Currency(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Currency")]
        [UIHint("Currency")]
        public string QuotaUnitPriceDisplay { get; set; }

        [SettingElement]
        public decimal? QuotaUnitPrice
        {
            get
            {
                decimal result;
                if (decimal.TryParse(QuotaUnitPriceDisplay, out result))
                    return result;
                return null;
            }
            set
            {
                QuotaUnitPriceDisplay = value.HasValue ? value.Value.ToString("###,##0.00") : null;
            }
        }

        public QuotaSettingsViewModel() { }

        public QuotaSettingsViewModel(bool loadUp)
        {
            if (loadUp)
            {
                QuotaUnit = QuotaSettings.QuotaUnit;
                QuotaUnitPrice = QuotaSettings.QuotaUnitPrice;
            }
        }
    }
}
