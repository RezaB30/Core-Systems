using RadiusR.DB;
using RezaB.DBUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class CallCenterSettingsViewModel
    {
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "APIKey")]
        [SettingElement]
        public string CallCenterAPIKey { get; set; }

        [SettingElement]
        public string CallCenterInternalIDs
        {
            get
            {
                return string.Join(",", InternalIDList.Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Replace(",", "").Trim().Substring(0, Math.Min(10, s.Length))));
            }
        }

        //[Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "InternalIDList")]
        public IEnumerable<string> InternalIDList { get; set; }

        public CallCenterSettingsViewModel() { }

        public CallCenterSettingsViewModel(bool loadUp)
        {
            if (loadUp)
            {
                CallCenterAPIKey = CallCenterSettings.CallCenterAPIKey;
                InternalIDList = new List<string>(CallCenterSettings.CallCenterInternalIDList);
            }
        }
    }
}
