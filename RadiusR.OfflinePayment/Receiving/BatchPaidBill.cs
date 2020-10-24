using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.OfflinePayment.Receiving
{
    public class BatchPaidBill
    {
        public string SubscriptionNo { get; set; }

        public string BillNo { get; set; }

        public decimal BillTotal { get; set; }

        public DateTime PayDate { get; set; }

        public DateTime DueDate { get; set; }
    }
}
