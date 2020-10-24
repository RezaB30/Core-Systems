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
    public class SchedulerSettingsViewModel
    {
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SchedulerStartTime")]
        [TimeSpan(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TimeSpan")]
        [SettingElement]
        public string SchedulerStartTime { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SchedulerStopTime")]
        [TimeSpan(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TimeSpan")]
        [SettingElement]
        public string SchedulerStopTime { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SchedulerRetryDelay")]
        [TimeSpan(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TimeSpan")]
        [SettingElement]
        public string SchedulerRetryDelay { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SMSSchedulerStartTime")]
        [TimeSpan(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TimeSpan")]
        [SettingElement]
        public string SMSSchedulerStartTime { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SMSSchedulerStopTime")]
        [TimeSpan(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TimeSpan")]
        [SettingElement]
        public string SMSSchedulerStopTime { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SMSSchedulerPaymentReminderThreshold")]
        [SettingElement]
        public int SMSSchedulerPaymentReminderThreshold { get; set; }

        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SMSSchedulerPrepaidReminderThreshold")]
        [SettingElement]
        public int SMSSchedulerPrepaidReminderThreshold { get; set; }

        public TimeSpan? _schedulerStartTime
        {
            get
            {
                TimeSpan result;
                if (TimeSpan.TryParse(SchedulerStartTime, out result))
                    return result;
                return null;
            }
            set
            {
                SchedulerStartTime = value.Value.ToString();
            }
        }

        public TimeSpan? _schedulerStopTime
        {
            get
            {
                TimeSpan result;
                if (TimeSpan.TryParse(SchedulerStopTime, out result))
                    return result;
                return null;
            }
            set
            {
                SchedulerStopTime = value.Value.ToString();
            }
        }

        public TimeSpan? _schedulerRetryDelay
        {
            get
            {
                TimeSpan result;
                if (TimeSpan.TryParse(SchedulerRetryDelay, out result))
                    return result;
                return null;
            }
            set
            {
                SchedulerRetryDelay = value.Value.ToString();
            }
        }

        public TimeSpan? _smsSchedulerStartTime
        {
            get
            {
                TimeSpan result;
                if (TimeSpan.TryParse(SMSSchedulerStartTime, out result))
                    return result;
                return null;
            }
            set
            {
                SMSSchedulerStartTime = value.Value.ToString();
            }
        }

        public TimeSpan? _smsSchedulerStopTime
        {
            get
            {
                TimeSpan result;
                if (TimeSpan.TryParse(SMSSchedulerStopTime, out result))
                    return result;
                return null;
            }
            set
            {
                SMSSchedulerStopTime = value.Value.ToString();
            }
        }
    }
}
