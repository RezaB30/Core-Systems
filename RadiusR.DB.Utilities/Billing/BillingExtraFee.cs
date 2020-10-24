using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Billing
{
    public class BillingExtraFee
    {
        public decimal Cost { get; set; }

        public FeeType FeeType { get; set; }

        public int? VariantID { get; set; }

        public bool IsAllTime { get; set; }
    }
}
