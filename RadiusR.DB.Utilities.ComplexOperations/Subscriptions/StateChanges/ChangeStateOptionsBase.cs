using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges
{
    public abstract class ChangeStateOptionsBase
    {
        protected static Lookup<CustomerState, CustomerState> ValidChanges = (Lookup<CustomerState, CustomerState>)new List<KeyValuePair<CustomerState, CustomerState>>()
        {
             new KeyValuePair<CustomerState, CustomerState>(CustomerState.PreRegisterd, CustomerState.Registered),
             new KeyValuePair<CustomerState, CustomerState>(CustomerState.PreRegisterd, CustomerState.Cancelled),
             new KeyValuePair<CustomerState, CustomerState>(CustomerState.Registered, CustomerState.Reserved),
             new KeyValuePair<CustomerState, CustomerState>(CustomerState.Registered, CustomerState.Cancelled),
             new KeyValuePair<CustomerState, CustomerState>(CustomerState.Reserved, CustomerState.Active),
             new KeyValuePair<CustomerState, CustomerState>(CustomerState.Reserved, CustomerState.Cancelled),
             new KeyValuePair<CustomerState, CustomerState>(CustomerState.Active, CustomerState.Disabled),
             new KeyValuePair<CustomerState, CustomerState>(CustomerState.Active, CustomerState.Cancelled),
             new KeyValuePair<CustomerState, CustomerState>(CustomerState.Disabled, CustomerState.Active)
        }.ToLookup(e => e.Key, e => e.Value);

        protected static Dictionary<CustomerState, bool> RadiusAuthorizationStateDictionary = new Dictionary<CustomerState, bool>() 
        {
            { CustomerState.Active, true },
            { CustomerState.Disabled, false },
            { CustomerState.Cancelled, false },
            { CustomerState.Registered, false },
            { CustomerState.Reserved, true },
            { CustomerState.Dismissed, false },
            { CustomerState.PreRegisterd, false },
        };

        public bool RadiusAuthorizationState
        {
            get
            {
                return RadiusAuthorizationStateDictionary[NewState];
            }
        }

        public int? AppUserID { get; set; }

        public SystemLogInterface LogInterface { get; set; }

        public string LogInterfaceUsername { get; set; }

        public bool ScheduleSMSes { get; set; } = false;

        public virtual CustomerState NewState { get; }

        internal bool IsValidChange(CustomerState oldState)
        {
            return ValidChanges.Contains(oldState) && ValidChanges[oldState].Any(s => s == NewState);
        }

        internal static IEnumerable<CustomerState> GetValidStateChanges(CustomerState oldState)
        {
            return ValidChanges.Contains(oldState) ? ValidChanges[oldState] : Enumerable.Empty<CustomerState>();
        }
    }
}
