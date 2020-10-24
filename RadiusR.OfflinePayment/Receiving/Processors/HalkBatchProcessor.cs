using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.OfflinePayment.Receiving.Processors
{
    class HalkBatchProcessor : IBatchPaymentProcessor
    {
        public IEnumerable<BatchPaidBill> ProcessBatch(Stream source)
        {
            string currentLine = string.Empty;
            try
            {
                source.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(source, Encoding.GetEncoding("ISO-8859-9"));
                var lines = reader.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var dataLines = lines.Where(l => l.StartsWith("D")).ToArray();
                var resultsList = new List<BatchPaidBill>();
                foreach (var line in dataLines)
                {
                    currentLine = line;
                    resultsList.Add(new BatchPaidBill()
                    {
                        SubscriptionNo = new string(line.Skip(1).Take(18).ToArray()).Trim(),
                        BillNo = new string(line.Skip(19).Take(20).ToArray()).Trim(),
                        DueDate = DateTime.ParseExact(new string(line.Skip(39).Take(8).ToArray()),"ddMMyyyy",System.Globalization.CultureInfo.InvariantCulture),
                        BillTotal = decimal.Parse(new string(line.Skip(47).Take(16).ToArray()).Trim().Replace(',','.'), System.Globalization.CultureInfo.InvariantCulture),
                        PayDate = DateTime.ParseExact(new string(line.Skip(63).Take(8).ToArray()), "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture),
                    });
                }

                return resultsList.ToArray();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("error at line -> " + currentLine, ex);
            }
        }
    }
}
