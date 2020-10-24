using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Binders
{
    public class InvariantCultureDecimalBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string key = bindingContext.ModelName;
            var result = bindingContext.ValueProvider.GetValue(key);
            if (result == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(result.AttemptedValue))
            {
                return null;
            }
            try
            {
                return new Models.ViewModels.PDFTemplates.PDFParametersViewModel.InvariantDecimal() { Value = decimal.Parse(result.AttemptedValue, CultureInfo.InvariantCulture) };
            }
            catch
            {
                return null;
            }
        }
    }
}