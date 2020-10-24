using RadiusR.DB.Enums.CustomerSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.DB.Enums;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges
{
    /// <summary>
    /// Options for changing subscription state to reserved.
    /// </summary>
    public class ReserveSubscriptionOptions : ChangeStateOptionsBase
    {
        public override CustomerState NewState
        {
            get
            {
                return CustomerState.Reserved;
            }
        }

        /// <summary>
        /// Setup task if applicable else null.
        /// </summary>
        public SetupRequest SetupServiceRequest { get; set; }

        /// <summary>
        /// Setup task options.
        /// </summary>
        public class SetupRequest
        {
            /// <summary>
            /// Setup task description.
            /// </summary>
            public string SetupTaskDescription { get; set; }
            /// <summary>
            /// Setup operator id.
            /// </summary>
            public int SetupUserID { get; set; }
            /// <summary>
            /// If subscriber has modem.
            /// </summary>
            public bool HasModem { get; set; }
            /// <summary>
            /// Subscriber modem type.
            /// </summary>
            public string ModemName { get; set; }
            /// <summary>
            /// Subscriber DSL type.
            /// </summary>
            public XDSLTypes XDSLType { get; set; }
        }
    }
}
