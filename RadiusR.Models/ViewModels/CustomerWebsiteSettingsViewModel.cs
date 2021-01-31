using RadiusR.DB;
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

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "WebsiteServicesInfrastructureDomain")]
        //[Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [SettingElement]
        public int? WebsiteServicesInfrastructureDomainID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "WebsiteServicesUsername")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(150, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [MinLength(6, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MinLength")]
        [SettingElement]
        public string WebsiteServicesUsername { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "WebsiteServicesPassword")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(150, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [MinLength(6, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MinLength")]
        [SettingElement]
        public string WebsiteServicesPassword { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "MaxSupportAttachmentSize")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [PositiveLong(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveLong")]
        [UIHint("TrafficLimit")]
        public string MaxSupportAttachmentSizeDisplay { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "MaxSupportAttachmentPerRequest")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [PositiveInt(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PositiveInt")]
        [MaxLength(3, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        public string MaxSupportAttachmentPerRequestDisplay { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "UseGoogleRecaptcha")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [SettingElement]
        public bool CustomerWebsiteUseGoogleRecaptcha { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "RecaptchaClientKey")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(150, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [SettingElement]
        public string CustomerWebsiteRecaptchaClientKey { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "RecaptchaServerKey")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(150, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [SettingElement]
        public string CustomerWebsiteRecaptchaServerKey { get; set; }

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

        [SettingElement]
        public long? MaxSupportAttachmentSize
        {
            get
            {
                long val = 0;
                if (long.TryParse(MaxSupportAttachmentSizeDisplay, out val))
                    return val;
                return null;
            }
            set
            {
                MaxSupportAttachmentSizeDisplay = value.ToString();
            }
        }

        [SettingElement]
        public int? MaxSupportAttachmentPerRequest
        {
            get
            {
                int val = 0;
                if (int.TryParse(MaxSupportAttachmentPerRequestDisplay, out val))
                    return val;
                return null;
            }
            set
            {
                MaxSupportAttachmentPerRequestDisplay = value.ToString();
            }
        }

        public CustomerWebsiteSettingsViewModel() { }

        public CustomerWebsiteSettingsViewModel(bool loadup = false)
        {
            if (loadup)
            {
                _OnlinePasswordDuration = CustomerWebsiteSettings.OnlinePasswordDuration;
                _SupportRequestPassedTime = CustomerWebsiteSettings.SupportRequestPassedTime;
                WebsiteServicesInfrastructureDomainID = CustomerWebsiteSettings.WebsiteServicesInfrastructureDomainID;
                WebsiteServicesUsername = CustomerWebsiteSettings.WebsiteServicesUsername;
                WebsiteServicesPassword = CustomerWebsiteSettings.WebsiteServicesPassword;
                MaxSupportAttachmentSize = CustomerWebsiteSettings.MaxSupportAttachmentSize;
                MaxSupportAttachmentPerRequest = CustomerWebsiteSettings.MaxSupportAttachmentPerRequest;
                CustomerWebsiteUseGoogleRecaptcha = CustomerWebsiteSettings.CustomerWebsiteUseGoogleRecaptcha;
                CustomerWebsiteRecaptchaClientKey = CustomerWebsiteSettings.CustomerWebsiteRecaptchaClientKey;
                CustomerWebsiteRecaptchaServerKey = CustomerWebsiteSettings.CustomerWebsiteRecaptchaServerKey;
            }
        }
    }
}
