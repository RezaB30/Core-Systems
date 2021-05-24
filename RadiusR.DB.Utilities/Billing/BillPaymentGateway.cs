using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Billing
{
    public class BillPaymentGateway
    {
        public RadiusRBillingService PaymentServiceUser { get; set; }

        public Partner PaymentPartner { get; set; }

        public PartnerSubUser PaymentPartnerSubUser { get; set; }

        public OfflinePaymentGateway OfflineGateway { get; set; }

        public Agent PaymentAgent { get; set; }
    }
}
