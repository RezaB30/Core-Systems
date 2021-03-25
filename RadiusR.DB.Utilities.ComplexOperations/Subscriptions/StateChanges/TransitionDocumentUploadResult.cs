using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges
{
    public class TransitionDocumentUploadResult
    {
        public bool IsSuccess
        {
            get
            {
                return ErrorResult == null;
            }
        }

        public StateChangeResult ErrorResult { get; private set; }

        public bool IsCritical { get; private set; }

        internal TransitionDocumentUploadResult(StateChangeResult stateChangeResult, bool isCritical)
        {
            ErrorResult = stateChangeResult;
            IsCritical = isCritical;
        }

        internal TransitionDocumentUploadResult() { IsCritical = false; }
    }
}
