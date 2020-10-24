using RadiusR_Manager.Models.CustomAttributes;
using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class ServiceRateTimePartitionViewModel
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Start")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [TimeOfDay(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TimeOfDay")]
        [UIHint("TimeOfDay")]
        public string StartTime { get; set; }

        public TimeSpan? _startTime
        {
            get
            {
                TimeSpan result;
                if (TimeSpan.TryParse(StartTime, CultureInfo.InvariantCulture, out result))
                    return result;
                return null;
            }
            set
            {
                StartTime = value.HasValue ? value.Value.ToString(@"hh\:mm") : null;
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "End")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [TimeOfDay(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "TimeOfDay")]
        [UIHint("TimeOfDay")]
        public string EndTime { get; set; }

        public TimeSpan? _endTime
        {
            get
            {
                TimeSpan result;
                if (TimeSpan.TryParse(EndTime, CultureInfo.InvariantCulture, out result))
                    return result;
                return null;
            }
            set
            {
                EndTime = value.HasValue ? value.Value.ToString(@"hh\:mm") : null;
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "RateLimit")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        public string RateLimit
        {
            get
            {
                return RateLimitView != null ? RateLimitView.ToString() : null;
            }
            set
            {
                RateLimitView = MikrotikRateLimitViewModel.Parse(value) ?? new MikrotikRateLimitViewModel();
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "RateLimit")]
        [Required(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "Required")]
        [RateLimit(ErrorMessageResourceType = typeof(RadiusR.Localization.Validation.Common), ErrorMessageResourceName = "RateLimit")]
        [UIHint("RateLimit")]
        public MikrotikRateLimitViewModel RateLimitView { get; set; }
    }
}
