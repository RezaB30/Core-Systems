using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class VPOSPaymentViewModel
    {
        public string RedirectUrl { get; set; }

        public IEnumerable<long> BillIds { get; set; }

        public long ClientId { get; set; }

        public int ExtendPeriodCount { get; set; }
    }
}