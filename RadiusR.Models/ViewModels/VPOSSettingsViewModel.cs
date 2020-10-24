using RadiusR.DB;
using RadiusR.DB.Enums;
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
    public class VPOSSettingsViewModel
    {
        public int ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.VPOSSettings.Common), Name = "VPOSID")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [EnumType(typeof(VPOSTypes), typeof(RadiusR.Localization.Lists.VPOSTypes))]
        [UIHint("LocalizedList")]
        public short? VPOSType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.VPOSSettings.Common), Name = "MerchantID")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string MerchantID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.VPOSSettings.Common), Name = "StoreKey")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string StoreKey { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.VPOSSettings.Common), Name = "Password")]
        //[Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string Password { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.VPOSSettings.Common), Name = "UserID")]
        //[Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string UserID { get; set; }

        public bool IsSelected { get; set; }
    }
}
