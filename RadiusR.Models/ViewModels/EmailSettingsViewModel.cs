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
    public class EmailSettingsViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SMTPEmailHost")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(150, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [SettingElement]
        public string SMTPEmailHost { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SMTPEMailPort")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(5, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [PortNumber(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "PortNumber")]
        [SettingElement]
        public string SMTPEMailPort { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SMTPEmailAddress")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Email(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "EmailValidation")]
        [MaxLength(150, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [SettingElement]
        public string SMTPEmailAddress { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SMTPEmailPassword")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(150, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [SettingElement]
        public string SMTPEmailPassword { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SMTPEmailDisplayName")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [MaxLength(150, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [SettingElement]
        public string SMTPEmailDisplayName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.AppSettings.Names), Name = "SMTPEmailDisplayEmail")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Email(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "EmailValidation")]
        [MaxLength(150, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "MaxLength")]
        [SettingElement]
        public string SMTPEmailDisplayEmail { get; set; }

        public int? _SMTPEMailPort
        {
            get
            {
                int result;
                if (int.TryParse(SMTPEMailPort, out result))
                    return result;
                return null;
            }
            set
            {
                SMTPEMailPort = value.Value.ToString();
            }
        }

        public EmailSettingsViewModel() { }

        public EmailSettingsViewModel(bool loadup = false)
        {
            if (loadup)
            {
                SMTPEmailHost = EmailSettings.SMTPEmailHost;
                _SMTPEMailPort = EmailSettings.SMTPEMailPort;
                SMTPEmailAddress = EmailSettings.SMTPEmailAddress;
                SMTPEmailPassword = EmailSettings.SMTPEmailPassword;
                SMTPEmailDisplayEmail = EmailSettings.SMTPEmailDisplayEmail;
                SMTPEmailDisplayName = EmailSettings.SMTPEmailDisplayName;
            }
        }
    }
}
