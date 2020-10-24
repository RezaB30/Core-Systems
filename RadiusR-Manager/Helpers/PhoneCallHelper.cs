using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Helpers
{
    public static class PhoneCallHelper
    {
        public static MvcHtmlString PhoneCallFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, object htmlAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var value = metadata.Model;

            TagBuilder link = new TagBuilder("a");
            link.AddCssClass("phone-call-link");
            link.MergeAttribute("href", "tel:+90" + value);
            link.SetInnerText("+90" + value);

            if (htmlAttributes != null)
            {
                foreach (var propertyName in htmlAttributes.GetType().GetProperties().Select(property => property.Name))
                {
                    link.MergeAttribute(propertyName, htmlAttributes.GetType().GetProperty(propertyName).GetValue(htmlAttributes).ToString());
                }
            }

            return new MvcHtmlString(link.ToString(TagRenderMode.Normal));
        }
    }
}