using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ClientUsageReportViewModel
    {

        public long ClientID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ClientName")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public string SubscriberNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PhoneNo")]
        public string PhoneNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Usage")]
        [UIHint("FormattedBytes")]
        public decimal Usage { get; set; }
    }
}
