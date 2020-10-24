using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.OfflinePayment.Sending.Formatters
{
    class HalkBatchFormatter : IBatchBillFormatter
    {
        public void FormatForUpload(Stream destination, IEnumerable<BatchReadyBill> data, FinishLine total = null, bool WriteHeaderLine = false)
        {
            var errorData = string.Empty;
            try
            {
                StreamWriter writer = new StreamWriter(destination, Encoding.GetEncoding("ISO-8859-9"));
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
                        item.SubscriberNo.PadRight(18),
                        (item.FullName.Length < 50 ? item.FullName.PadRight(50) : item.FullName.Substring(0, 50)),
                        item.BillNo.PadLeft(15, '0').PadRight(20),
                        item.DueDate.ToString("ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture),
                        item.Amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).Replace('.', ',').PadLeft(16, '0')
                    });

                    writer.WriteLine(currentRow);
                }
                // last line if available
                if (total != null)
                {
                    string footer = string.Join("", new string[]
                    {
                        "F",
                        total.TotalCount.ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(15, '0'),
                        Math.Round(total.TotalAmount, 2, MidpointRounding.AwayFromZero).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).Replace('.', ',').PadLeft(16, '0'),
                        DateTime.Now.ToString("yyyyMMddHHmm")
                    });

                    writer.Write(footer);
                }
                // flush
                writer.Flush();
                //writer.Close();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error processing data -> " + errorData, ex);
            }
        }

        public string GetUploadFileName(DateTime date)
        {
            return "BORC" + date.ToString("yyMMdd", System.Globalization.CultureInfo.InvariantCulture) + ".TXT";
        }
    }
}
