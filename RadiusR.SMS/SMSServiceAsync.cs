using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.DB;
using RadiusR.DB.Enums;
using System.Threading;

namespace RadiusR.SMS
{
    [Obsolete("Use non Async method")]
    public class SMSServiceAsync : SMSService
    {
        protected override SMSArchive Send(Subscription subscription, string message, long? billId = default(long?), SMSType? type = default(SMSType?))
        {
            var thread = new Thread(new ThreadStart(() => 
            {
                var isSent = Execute(subscription.Customer.ContactPhoneNo, message, billId, type);

            }));
            thread.Start();
            return new SMSArchive()
            {
                SubscriptionID = subscription.ID,
                BillID = billId,
                SMSTypeID = (short?)type,
                Date = DateTime.Now,
                Text = message
            };
        }
    }
}
