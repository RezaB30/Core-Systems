using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TariffChanges
{
    public enum TariffChangeResult
    {
        TariffChanged,
        TariffChangeScheduled,
        InvalidInput,
        InvalidDomain,
        InvalidSubscriptionState
    }
}
