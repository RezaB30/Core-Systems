using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.Registration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class PositiveIntegerAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is int)
            {
                if ((int)value > 0)
                    return ValidationResult.Success;

                return new ValidationResult(string.Format(ErrorMessageString, validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }
    }
}
