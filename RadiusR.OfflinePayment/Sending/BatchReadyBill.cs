using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.OfflinePayment.Sending
{
    public class BatchReadyBill
    {
        public string BillNo { get; set; }
        public string SubscriberNo { get; set; }
        public string FullName { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
    }
}
