using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Billing
{
    public static class BillFeeExtentions
    {
        public static decimal GetTaxBase(this BillFee billFee)
        {
            var feeType = billFee.FeeTypeCost ?? billFee.Fee.FeeTypeCost;
            return (billFee.CurrentCost - (billFee.Discount != null ? billFee.Discount.Amount : 0m )) / (feeType.TaxRates.Sum(taxRate => taxRate.Rate) + 1m);
        }
    }
}
