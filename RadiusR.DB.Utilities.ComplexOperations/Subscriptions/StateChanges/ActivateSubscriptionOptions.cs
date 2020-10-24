using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.DB.Enums;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges
{
    public class ActivateSubscriptionOptions : ChangeStateOptionsBase
    {
        public override CustomerState NewState
        {
            get
            {
                return CustomerState.Active;
            }
        }

        public bool ForceUnfreeze { get; set; }
    }
}
