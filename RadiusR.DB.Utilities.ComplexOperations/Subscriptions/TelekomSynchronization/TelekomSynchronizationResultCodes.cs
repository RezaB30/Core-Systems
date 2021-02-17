using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TelekomSynchronization
{
    public enum TelekomSynchronizationResultCodes
    {
        Success = 0,
        TelekomError = 1,
        InvalidDomain = 2,
        SubscriptionHasNoTelekomInfo = 3,
        SynchronizedUsernameExists = 4,
        InvalidOptions = 5,
    }
}
