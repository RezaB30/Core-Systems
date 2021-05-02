using RezaB.Web.Helpers.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Binders
{
    public class DateWithTimeBinder: IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string key = bindingContext.ModelName;
            var result = bindingContext.ValueProvider.GetValue(key);
            if (result == null)
            {
                return null;
            }

            if (bindingContext.ModelType == typeof(DateWithTime) && string.IsNullOrWhiteSpace(result.AttemptedValue))
            {
                return null;
            }

            DateTime value;

            if (DateTime.TryParseExact(result.AttemptedValue, "MM/dd/yyyy HH:mm", Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out value))
            {
                return new DateWithTime() { InternalValue = value };
            }

            bindingContext.ModelState.AddModelError(key, string.Format(RadiusR.Localization.Validation.Common.DateBinder, bindingContext.ModelMetadata.DisplayName));

            return null;
        }
    }
}