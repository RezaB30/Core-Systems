using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TelekomSynchronization
{
    /// <summary>
    /// Options to use for telekom synchronization.
    /// </summary>
    public class TelekomSynchronizationOptions
    {
        /// <summary>
        /// Application user id.
        /// </summary>
        public int? AppUserID { get; set; }
        /// <summary>
        /// Logging interface.
        /// </summary>
        public SystemLogInterface? LogInterface { get; set; }
        /// <summary>
        /// The target subscription from database context.
        /// </summary>
        public Subscription DBSubscription { get; set; }
        /// <summary>
        /// The XDSL no to synchronize the subscription with.
        /// </summary>
        public string DSLNo { get; set; }
    }
}
