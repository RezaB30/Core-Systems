using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Helpers
{
    public static class RoleDisplayHelper
    {
        public static MvcHtmlString RoleDisplay<TModel,TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel,TResult>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var value = metadata.Model as RadiusR.DB.Role;
            object _enumType;
            object _resourceType;
            Type enumType;
            Type resourceType;
            if (metadata.AdditionalValues.TryGetValue("EnumType", out _enumType) && metadata.AdditionalValues.TryGetValue("EnumResourceType", out _resourceType))
            {
                enumType = _enumType as Type;
                if (!Enum.IsDefined(enumType, value.ID))
                {
                    return new MvcHtmlString(value.Name);
                }
                resourceType = _resourceType as Type;
                var type = typeof(LocalizedList<,>).MakeGenericType(enumType, resourceType);
                var genericList = Activator.CreateInstance(type) as LocalizedList;

                return new MvcHtmlString(genericList.GetDisplayText((short)value.ID));
            }

            return new MvcHtmlString(value.Name);
        }
    }
}