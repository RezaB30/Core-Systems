using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.OfflinePayment.Sending.Formatters
{
    class FinansBatchFormatter : IBatchBillFormatter
    {
        public void FormatForUpload(Stream destination, IEnumerable<BatchReadyBill> data, FinishLine total = null, bool WriteHeaderLine = false)
        {
            var errorData = string.Empty;
            try
            {
                StreamWriter writer = new StreamWriter(destination, Encoding.GetEncoding("ISO-8859-9"));

                // header data
                if (WriteHeaderLine)
                {
                    string header = "H" + DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                    writer.WriteLine(header);
                }
                // iterating data
                foreach (var item in data)
                {
                    errorData = string.Join("|", new string[]
                    {
                        item.BillNo,
                        item.FullName,
                        item.SubscriberNo ,
                        item.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        item.DueDate.ToShortDateString()
                    });

                    var currentRow = string.Join("", new string[]
                    {
                        "D",
                        item.SubscriberNo,
                        (item.FullName.Length < 30 ? item.FullName.PadRight(30) : item.FullName.Substring(0, 30)),
                        item.DueDate.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                        "Y",
                        item.BillNo.PadLeft(15, '0'),
                        item.Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).PadLeft(15, '0')
                    });

                    writer.WriteLine(currentRow);
                }
                // last line if available
                if (total != null)
                {
                    string footer = string.Join("", new string[]
                    {
                        "T",
                        total.TotalCount.ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(7, '0'),
                        Math.Round(total.TotalAmount, 2, MidpointRounding.AwayFromZero).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).PadLeft(16, '0')
                    });

                    writer.Write(footer);
                }
                // flush
                writer.Flush();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error processing data -> " + errorData, ex);
            }
        }

        public string GetUploadFileName(DateTime date)
        {
            return "BRC" + date.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) + "1.txt";
        }
    }
}
