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

        //[Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DailyDisconnectionTime")]
        //[Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        //[TimeSpan(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TimeSpan")]
        //public string DailyDisconnectionTime
        //{
        //    get
        //    {
        //        return _dailyDisconnectionTime.ToString("hh\\:mm\\:ss");
        //    }
        //    set
        //    {
        //        TimeSpan parsed;
        //        if (TimeSpan.TryParseExact(value, "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out parsed))
        //        {
        //            _dailyDisconnectionTime = parsed;
        //        }
        //    }
        //}

        //private TimeSpan _dailyDisconnectionTime;

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IncludeICMP")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public bool IncludeICMP { get; set; }

        public string _includeICMP
        {
            get
            {
                return IncludeICMP.ToString();
            }
            set
            {
                bool parsed;
                if (bool.TryParse(value, out parsed))
                {
                    IncludeICMP = parsed;
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