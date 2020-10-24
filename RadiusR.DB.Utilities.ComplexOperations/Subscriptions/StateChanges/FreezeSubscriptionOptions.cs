using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.DB.Enums;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges
{
    public class FreezeSubscriptionOptions : ChangeStateOptionsBase
    {
        public override CustomerState NewState
        {
            get
            {
                return CustomerState.Disabled;
            }
        }

        public DateTime ReleaseDate { get; set; }

        public bool ForceThroughWebService { get; set; }
    }
}
