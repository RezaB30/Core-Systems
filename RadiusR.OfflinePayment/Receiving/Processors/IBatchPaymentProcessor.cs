using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.OfflinePayment.Receiving.Processors
{
    interface IBatchPaymentProcessor
    {
        IEnumerable<BatchPaidBill> ProcessBatch(Stream source);
    }
}
