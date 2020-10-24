using RadiusR.DB;
using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Extentions
{
    public static class ClientCancellationUtilities
    {
        public static void AddOrUpdateClientCancellation(this RadiusREntities entities, long subscriptionId, CancellationReason reason, string text)
        {
            var current = entities.SubscriptionCancellations.Find(subscriptionId);
            if (current != null)
            {
                current.ReasonID = (short)reason;
                current.ReasonText = text;
            }
            else
            {
                entities.SubscriptionCancellations.Add(new SubscriptionCancellation()
                {
                    SubscriptionID = subscriptionId,
                    ReasonID = (short)reason,
                    ReasonText = text
                });
            }
        }
    }
}
