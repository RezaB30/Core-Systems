using RezaB.Radius.Packet.AttributeEnums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class RadiusSettingsViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "InterimInterval")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Number(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Number")]
        [Range(10, 300, ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "IntRange")]
        public string AccountingInterimInterval
        {
            get
            {
                return _accountingInterimInterval.ToString();
            }
            set
            {
                int parsed;
                if (int.TryParse(value, out parsed))
                {
                    _accountingInterimInterval = parsed;
                }
            }
        }

        private int _accountingInterimInterval;

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Protocol")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Number(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Number")]
        [EnumType(typeof(FramedProtocol), typeof(RadiusR.Localization.Lists.FramedProtocol))]
        [UIHint("LocalizedList")]
        public short FramedProtocol { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CheckCLID")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public bool CheckCLID { get; set; }

        public string _checkCLID
        {
            get
            {
                return CheckCLID.ToString();
            }
            set
            {
                bool parsed;
                if (bool.TryParse(value, out parsed))
                {
                    CheckCLID = parsed;
                }
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RadiusSettingsRefreshInterval")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [TimeSpan(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TimeSpan")]
        public string RadiusSettingsRefreshInterval { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "NASListRefreshInterval")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [TimeSpan(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TimeSpan")]
        public string NASListRefreshInterval { get; set; }
    }
}