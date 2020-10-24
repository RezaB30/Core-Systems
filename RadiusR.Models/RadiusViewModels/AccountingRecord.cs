using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class AccountingRecord
    {
        public long ID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "NasName")]
        public string NasName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "StartDate")]
        [UIHint("ExactTime")]
        public DateTime StartTime { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "EndDate")]
        [UIHint("ExactTime")]
        public DateTime? StopTime
        {
            get
            {
                return IsOnline ? _updateTime : _stopTime;
            }
        }

        public DateTime? _updateTime { get; set; }

        public DateTime? _stopTime { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Connection")]
        public bool IsOnline
        {
            get
            {
                return _stopTime == null;
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LocalIP")]
        public string LocalIP { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RealIP")]
        public string RealIP { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CallingStation")]
        public string CallingStation { get; set; }
    }
}