using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class IPDRReportViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "Username")]
        public string Username { get; set; }

        public long SubscriptionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.FreeRadius), Name = "NasName")]
        public string NASName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "StartDate")]
        [UIHint("ExactTime")]
        public DateTime StartDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "EndDate")]
        [UIHint("ExactTime")]
        public DateTime? EndDate
        {
            get
            {
                return _endDate ?? _updateDate;
            }
        }

        public DateTime? _updateDate { get; set; }

        public DateTime? _endDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CallingStation")]
        public string CallingStation { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "RealIP")]
        public string RealIP { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LocalIP")]
        public string LocalIP { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PortRange")]
        public string PortRange { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SessionID")]
        public string SessionID { get; set; }
    }
}
