using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR_Manager.Models.RadiusViewModels;
using System.ComponentModel.DataAnnotations;

namespace RadiusR_Manager.Models.ViewModels
{
    public class SystemLogsReportViewModel : SystemLogViewModel
    {
        [Display(ResourceType =typeof(RadiusR.Localization.Model.RadiusR), Name = "Subscribers")]
        public IEnumerable<Subscriber> Subscribers { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ClientName")]
        public string CustomerName { get; set; }

        public class Subscriber
        {
            public long ID { get; set; }

            public string SubscriberNo { get; set; }
        }
    }
}
