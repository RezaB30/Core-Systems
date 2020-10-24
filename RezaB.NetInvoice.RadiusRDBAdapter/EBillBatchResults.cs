using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RezaB.NetInvoice.RadiusRDBAdapter
{
    public class EBillBatchResults
    {
        public int SuccessfulCount { get; internal set; }

        public int UnsuccessfulCount { get; internal set; }

        public int TotalCount { get; internal set; }

        public int InvalidCount
        {
            get
            {
                return TotalCount - SuccessfulCount - UnsuccessfulCount;
            }
        }

        public ResultType ErrorCode { get; set; }

        public EBillBatchResults()
        {
            SuccessfulCount = UnsuccessfulCount = TotalCount = 0;
            ErrorCode = ResultType.Success;
        }

        public enum ResultType
        {
            Success = 0,
            CuncurrencyDetected = 1,
            FatalError = 2,
            PartialError = 3
        }
    }
}
