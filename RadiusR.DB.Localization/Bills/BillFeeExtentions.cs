using RadiusR.DB.Enums;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RadiusR.DB.Localization.Bills
{
    public static class BillFeeExtentions
    {
        public static string GetDisplayName(this BillFee billFee, bool useClientCulture = false)
        {
            var culture = useClientCulture ? CultureInfo.CreateSpecificCulture(billFee.Bill.Subscription.Customer.Culture) : Thread.CurrentThread.CurrentCulture;

            var results = new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetDisplayText(billFee.FeeTypeID ?? billFee.Fee.FeeTypeID, culture);
            if (billFee.Fee != null && billFee.Fee.FeeTypeVariant != null)
                results += " (" + billFee.Fee.FeeTypeVariant.Title + ")";
            if (billFee.Fee != null && billFee.Fee.Description != null)
                results += " (" + billFee.Fee.Description + ")";
            if (!string.IsNullOrEmpty(billFee.Description))
                results += " (" + billFee.Description + ")";

            return results;
        }
    }
}
