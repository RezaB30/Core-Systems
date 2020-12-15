using RadiusR.DB.Settings;
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
    public class CustomerWebsiteSettingsViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "OnlinePasswordDuration")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [TimeSpan(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TimeSpan")]
        [SettingElement]
        public string OnlinePasswordDuration { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SupportRequestReopenAllowTime")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [TimeSpan(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TimeSpan")]
        [SettingElement]
        public string SupportRequestPassedTime { get; set; }

        public TimeSpan? _OnlinePasswordDuration
        {
            get
            {
                TimeSpan result;
                if (TimeSpan.TryParse(OnlinePasswordDuration, out result))
                    return result;
                return null;
            }
            set
            {
                OnlinePasswordDuration = value.Value.ToString();
            }
        }

        public TimeSpan? _SupportRequestPassedTime
        {
            get
            {
                TimeSpan result;
                if (TimeSpan.TryParse(SupportRequestPassedTime, out result))
                    return result;
                return null;
            }
            set
            {
                SupportRequestPassedTime = value.Value.ToString();
            }
        }

        public CustomerWebsiteSettingsViewModel() { }

        public CustomerWebsiteSettingsViewModel(bool loadup = false)
        {
            if (loadup)
            {
                _OnlinePasswordDuration = CustomerWebsiteSettings.OnlinePasswordDuration;
                _SupportRequestPassedTime = CustomerWebsiteSettings.SupportRequestPassedTime;
            }
        }
    }
}
