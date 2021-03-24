using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges
{
    /// <summary>
    /// Represents the result of a state change operation
    /// </summary>
    public class StateChangeResult
    {
        /// <summary>
        /// Determines if the operation finished successfuly.
        /// </summary>
        public bool IsSuccess { get; private set; }
        /// <summary>
        /// Determines if a fatal error occured.
        /// </summary>
        public bool IsFatal { get; private set; }
        /// <summary>
        /// The error message if operation was unsuccessful.
        /// </summary>
        public string ErrorMessage { get; private set; }
        /// <summary>
        /// The internal exception thrown.
        /// </summary>
        public Exception InternalException { get; private set; }
        /// <summary>
        /// Creates a success result.
        /// </summary>
        public StateChangeResult()
        {
            IsSuccess = true;
        }
        /// <summary>
        /// Creates a non-fatal error result.
        /// </summary>
        /// <param name="errorMessage">The description of error.</param>
        /// <param name="exception">The internal exception thrown (optional).</param>
        public StateChangeResult(string errorMessage, Exception exception = null)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
            InternalException = exception;
        }
        /// <summary>
        /// Creates a fatal error result.
        /// </summary>
        /// <param name="fatalException">The internal exception thrown.</param>
        public StateChangeResult(Exception fatalException)
        {
            IsSuccess = false;
            IsFatal = true;
            InternalException = fatalException;
        }
    }
}
