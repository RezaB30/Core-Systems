using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RezaB.NetInvoice.RadiusRDBAdapter
{
    public class CancellationResult
    {
        public CancellationResult(CancellationResultType resultType, string errorDescription = null)
        {
            ResultType = resultType;
            ErrorDescription = errorDescription;
        }

        public CancellationResultType ResultType { get; private set; }

        public string ErrorDescription { get; private set; }
    }

    public enum CancellationResultType
    {
        Success = 0,
        InvalidBillState = 1,
        WebServiceError = 2,
        InvalidEBillType = 3
    }
}
