using RezaB.TurkTelekom.WebServices.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TelekomSynchronization
{
    public class TelekomSynchronizationResults
    {
        public TelekomSynchronizationResultCodes ResultCode { get; set; }

        public ICollection<TTWebServiceException> TelekomExceptions { get; set; }

        public string SynchronizedUsername { get; set; }
    }
}
