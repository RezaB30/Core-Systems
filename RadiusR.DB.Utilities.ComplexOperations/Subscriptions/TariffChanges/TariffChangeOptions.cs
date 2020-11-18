using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TariffChanges
{
    public class TariffChangeOptions
    {
        public long SubscriptionID { get; set; }

        public int TariffID { get; set; }

        public short NewBillingPeriod { get; set; }

        public bool ForceNow { get; set; }

        public GatewayOptions Gateway { get; set; }
    }

    public class GatewayOptions
    {
        public int? UserID { get; set; }

        public SystemLogInterface? InterfaceType { get; set; }

        public string InterfaceUsername { get; set; }


    }
}
