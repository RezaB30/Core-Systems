using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges
{
    public class RegisterSubscriptionOptions : ChangeStateOptionsBase
    {
        public override CustomerState NewState
        {
            get
            {
                return CustomerState.Registered;
            }
        }
    }
}
