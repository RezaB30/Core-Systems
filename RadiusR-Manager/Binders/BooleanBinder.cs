using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Binders
{
    public class BooleanBinder: IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string key = bindingContext.ModelName;
            var result = bindingContext.ValueProvider.GetValue(key);
            if (result == null)
            {
                return false;
            }

            try
            {
                return result.AttemptedValue == "on";
            }
            catch
            {
                return null;
            }
        }
    }
}