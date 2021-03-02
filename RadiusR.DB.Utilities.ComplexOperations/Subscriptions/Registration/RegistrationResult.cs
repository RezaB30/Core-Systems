using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.Registration
{
    public class RegistrationResult
    {
        public ILookup<string, string> ValidationMessages { get; set; }

        public bool IsSuccess
        {
            get
            {
                return ValidationMessages == null;
            }
        }

        public bool? DoesCustomerExist { get; set; }
    }
}
