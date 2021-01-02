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
    public class FileManagerSettingsViewModel
    {
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FileManagerType")]
        [EnumType(typeof(RadiusR.DB.Enums.FileManagerTypes), typeof(RadiusR.Localization.Lists.FileManagerTypes))]
        [UIHint("LocalizedList")]
        [SettingElement]
        public short FileManagerType { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FileManagerHost")]
        [SettingElement]
        public string FileManagerHost { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Username")]
        [SettingElement]
        public string FileManagerUsername { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Password")]
        [SettingElement]
        public string FileManagerPassword { get; set; }

        public FileManagerSettingsViewModel() { }

        public FileManagerSettingsViewModel(bool loadup = false)
        {
            if (loadup)
            {
                FileManagerType = (short)FileManagerSettings.FileManagerType;
                FileManagerHost = FileManagerSettings.FileManagerHost;
                FileManagerUsername = FileManagerSettings.FileManagerUsername;
                FileManagerPassword = FileManagerSettings.FileManagerPassword;
            }
        }
    }
}
