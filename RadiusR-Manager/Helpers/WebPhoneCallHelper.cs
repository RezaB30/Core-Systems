using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR_Manager.Helpers
{
    public static class WebPhoneCallHelper
    {
        public static MvcHtmlString WebPhoneCallFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, string countryCode, object htmlAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var value = metadata.Model as string;
            UrlHelper Url = new UrlHelper(helper.ViewContext.RequestContext);

            TagBuilder link = new TagBuilder("a");
            link.AddCssClass("phone-call-link");
            link.MergeAttribute("href", "javascript: CallCenterCall('" + value + "','" + Url.Action("CallNumber", "CallCenter") + "');");
            link.SetInnerText(countryCode + value);

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