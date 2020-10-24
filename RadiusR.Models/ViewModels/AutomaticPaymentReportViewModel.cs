using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class AutomaticPaymentReportViewModel
    {
        public long SubscriptionID { get; set; }

        public string SubscriberNo { get; set; }

        public string Gateway { get; set; }
    }
}
