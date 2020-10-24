using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using RezaB.Web.Helpers;
using RezaB.Data.Localization;

namespace RadiusR_Manager.Helpers
{
    public static class LocalizedListEditorHelper
    {
        public static MvcHtmlString LocalizedListEditor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, object htmlAttributes = null, string defaultText = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);

            return helper.LocalizedListEditor(expression, fullName, htmlAttributes, defaultText);
        }

        public static MvcHtmlString LocalizedListEditor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, string id, object htmlAttributes = null, string defaultText = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            object _enumType;
            object _resourceType;
            Type enumType;
            Type resourceType;
            if (metadata.AdditionalValues.TryGetValue("EnumType", out _enumType) && metadata.AdditionalValues.TryGetValue("EnumResourceType", out _resourceType))
            {
                enumType = _enumType as Type;
                resourceType = _resourceType as Type;
                var type = typeof(LocalizedList<,>).MakeGenericType(enumType, resourceType);
                var genericList = Activator.CreateInstance(type) as LocalizedList;

                var items = genericList.GenericList;
                var selectList = new SelectList(items, "ID", "Name", metadata.Model != null && Convert.ToInt32(metadata.Model) > 0 ? metadata.Model : null);

                return helper.Select(id, selectList, defaultText != null ? defaultText : RadiusR.Localization.Helpers.Common.Select, htmlAttributes: htmlAttributes);
            }

            return new MvcHtmlString(metadata.Model.ToString());
        }
    }
}