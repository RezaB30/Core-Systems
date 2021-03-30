using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges
{
    public class TransitionRegistrationResult : StateChangeResult
    {
        /// <summary>
        /// Successfuly created transaction id.
        /// </summary>
        public long? TransactionID { get; protected set; }

        private TransitionRegistrationResult() { }
        /// <summary>
        /// Creates a success result with created transaction id.
        /// </summary>
        /// <param name="transactionId">Created transaction id.</param>
        public TransitionRegistrationResult(long transactionId)
        {
            TransactionID = transactionId;
        }

        /// <summary>
        /// Creates a fatal error result.
        /// </summary>
        /// <param name="fatalException">The internal exception thrown.</param>
        public TransitionRegistrationResult(Exception fatalException) : base(fatalException) { }

        /// <summary>
        /// Creates a non-fatal error result.
        /// </summary>
        /// <param name="errorMessage">The description of error.</param>
        /// <param name="exception">The internal exception thrown (optional).</param>
        public TransitionRegistrationResult(string errorMessage, Exception exception = null) : base(errorMessage, exception) { }
    }
}
