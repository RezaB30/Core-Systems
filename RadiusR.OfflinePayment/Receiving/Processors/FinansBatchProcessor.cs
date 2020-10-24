using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.OfflinePayment.Receiving.Processors
{
    class FinansBatchProcessor : IBatchPaymentProcessor
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
                        SubscriptionNo = new string(line.Skip(1).Take(10).ToArray()).Trim(),
                        DueDate = DateTime.ParseExact(new string(line.Skip(41).Take(8).ToArray()), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
                        BillNo = new string(line.Skip(50).Take(15).ToArray()).Trim(),
                        BillTotal = decimal.Parse(new string(line.Skip(65).Take(15).ToArray()).Trim(), System.Globalization.CultureInfo.InvariantCulture),
                        PayDate = DateTime.ParseExact(new string(line.Skip(80).Take(8).ToArray()), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture),
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
