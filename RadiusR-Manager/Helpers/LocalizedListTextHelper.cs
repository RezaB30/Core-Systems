using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Helpers
{
    public static class LocalizedListTextHelper
    {
        public static MvcHtmlString LocalizedListText<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            object _enumType;
            object _resourceType;
            Type enumType;
            Type resourceType;
            if( metadata.AdditionalValues.TryGetValue("EnumType", out _enumType) && metadata.AdditionalValues.TryGetValue("EnumResourceType", out _resourceType))
            {
                enumType = _enumType as Type;
                resourceType = _resourceType as Type;
                var type = typeof(LocalizedList<,>).MakeGenericType(enumType, resourceType);
                var genericList = Activator.CreateInstance(type) as LocalizedList;

                return new MvcHtmlString(genericList.GetDisplayText(metadata.Model != null ? Convert.ToInt32(metadata.Model) : (int?)null));
            }

            return new MvcHtmlString(metadata.Model.ToString());
        }
    }
}