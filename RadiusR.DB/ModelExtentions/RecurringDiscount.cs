using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public partial class RecurringDiscount
    {
        public RecurringDiscount(SpecialOffer specialOffer)
        {
            Amount = specialOffer.Amount;
            ApplicationTimes = specialOffer.ApplicationTimes;
            ApplicationType = specialOffer.ApplicationType;
            CreationTime = DateTime.Now;
            Description = specialOffer.Name;
            FeeTypeID = specialOffer.FeeTypeID;
            DiscountType = specialOffer.DiscountType;
            IsDisabled = false;
            OnlyFullInvoice = specialOffer.OnlyFullInvoice;
        }
    }
}
