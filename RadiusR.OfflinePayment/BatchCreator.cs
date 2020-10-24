using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.OfflinePayment.Sending;
using RadiusR.OfflinePayment.Sending.Formatters;
using RadiusR.OfflinePayment.Receiving;
using RadiusR.OfflinePayment.Receiving.Processors;

namespace RadiusR.OfflinePayment
{
    public static class BatchProcessor
    {
        public static void CopyToStream(Stream destination, FormatTypes format, IEnumerable<BatchReadyBill> data, FinishLine total = null, bool WriteHeaderLine = false)
        {
            IBatchBillFormatter formatter;
            switch (format)
            {
                case FormatTypes.Halkbank:
                    formatter = new HalkBatchFormatter();
                    break;
                case FormatTypes.Finansbank:
                    formatter = new FinansBatchFormatter();
                    break;
                default:
                    return;
            }

            formatter.FormatForUpload(destination, data, total, WriteHeaderLine);
        }

        public static IEnumerable<BatchPaidBill> ProcessStream(Stream source, FormatTypes format)
        {
            IBatchPaymentProcessor processor;
            switch (format)
            {
                case FormatTypes.Halkbank:
                    processor = new HalkBatchProcessor();
                    break;
                case FormatTypes.Finansbank:
                    processor = new FinansBatchProcessor();
                    break;
                default:
                    return null;
            }

            return processor.ProcessBatch(source);
        }

        public static string GetUploadFileName(DateTime date, FormatTypes format)
        {
            IBatchBillFormatter formatter;
            switch (format)
            {
                case FormatTypes.Halkbank:
                    formatter = new HalkBatchFormatter();
                    break;
                case FormatTypes.Finansbank:
                    formatter = new FinansBatchFormatter();
                    break;
                default:
                    return null;
            }

            return formatter.GetUploadFileName(date);
        }
    }
}
