using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Billing.AgentPayments
{
    public static class AgentPaymentTypes
    {
        public static IEnumerable<PaymentType> CommissionedPayments
        {
            get
            {
                return new[]
                {
                    PaymentType.MobilExpress,
                    PaymentType.VirtualPos,
                    PaymentType.PhysicalPos
                };
            }
        }

        public static IEnumerable<PaymentType> NegativeAllowancePayments
        {
            get
            {
                return new[]
                {
                    PaymentType.Cash
                };
            }
        }
    }
}
