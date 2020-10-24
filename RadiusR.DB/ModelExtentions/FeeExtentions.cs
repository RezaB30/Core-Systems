using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public partial class Fee
    {
        public bool CanBeCancelled
        {
            get
            {
                return !IsCancelled && !BillFees.Any(bf => bf.Bill.BillStatusID != (short)Enums.BillState.Cancelled);
            }
        }
    }
}
