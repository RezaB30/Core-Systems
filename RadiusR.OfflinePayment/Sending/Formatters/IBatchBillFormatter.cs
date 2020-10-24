using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.OfflinePayment.Sending.Formatters
{
    interface IBatchBillFormatter
    {
        void FormatForUpload(Stream destination, IEnumerable<BatchReadyBill> data, FinishLine total = null, bool WriteHeaderLine = false);

        string GetUploadFileName(DateTime date);
    }
}
