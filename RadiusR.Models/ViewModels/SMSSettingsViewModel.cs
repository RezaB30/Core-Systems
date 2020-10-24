using RadiusR.DB;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class SMSSettingsViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CachingLength")]
        [TimeSpan(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TimeSpan")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string CachingLength { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ServiceUsername")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string ServiceUsername { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ServicePassword")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string ServicePassword { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ServiceTitle")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string ServiceTitle { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IsActive")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public bool IsActive { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "APIType")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [EnumType(typeof(RadiusR.DB.Enums.SMSAPITypes), typeof(RadiusR.Localization.Lists.SMSAPIType))]
        [UIHint("LocalizedList")]
        public short APIType { get; set; }

        public void UpdateDatabase()
        {
            using (RadiusREntities db = new RadiusREntities())
            {
                // retrieve settings
                var dbSettings = db.SMSSettings.ToList();
                var cachingLength = dbSettings.FirstOrDefault(setting => setting.Key == "CachingLength");
                var serviceUsername = dbSettings.FirstOrDefault(setting => setting.Key == "ServiceUsername");
                var servicePassword = dbSettings.FirstOrDefault(setting => setting.Key == "ServicePassword");
                var serviceTitle = dbSettings.FirstOrDefault(setting => setting.Key == "ServiceTitle");
                var isActive = dbSettings.FirstOrDefault(setting => setting.Key == "IsActive");
                var apiType = dbSettings.FirstOrDefault(setting => setting.Key == "API");
                // update settings
                cachingLength.Value = CachingLength;
                serviceUsername.Value = ServiceUsername;
                servicePassword.Value = ServicePassword;
                serviceTitle.Value = ServiceTitle;
                isActive.Value = IsActive.ToString();
                apiType.Value = APIType.ToString();
                foreach (var item in dbSettings)
                {
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }
        }
    }
}