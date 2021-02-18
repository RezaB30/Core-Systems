using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.DB.Enums;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges
{
    public class CancelSubscriptionOptions : ChangeStateOptionsBase
    {
        public override CustomerState NewState
        {
            get
            {
                return IsDismissed ? CustomerState.Dismissed : CustomerState.Cancelled;
            }
        }

        public CancellationReason CancellationReason { get; set; }

        public string CancellationReasonDescription { get; set; }

        public bool ForceCancellation { get; set; }

        internal bool IsDismissed { get; set; }

        public bool DoNotCancelTelekomService { get; set; }
    }
}
