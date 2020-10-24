using RadiusR.DB.Enums;
using RadiusR.DB.Enums.RecurringDiscount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Discounts
{
    public class DiscountOperationOptions
    {
        public int? AppUserID { get; set; }

        public SystemLogInterface LogInterface { get; set; }

        public string LogInterfaceUsername { get; set; }

        public RecurringDiscountCancellationCause CancellationCause { get; set; }
    }
}
