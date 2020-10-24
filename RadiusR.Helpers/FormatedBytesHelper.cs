using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RadiusR.Helpers
{
    public static class FormatedBytesHelper
    {
        public static MvcHtmlString FormattedBytes(this HtmlHelper helper, string value)
        {
            TagBuilder span = new TagBuilder("span");
            if (!string.IsNullOrEmpty(value))
            {
                var mixedValue = value.Split(' ');
                if (mixedValue.Length != 2)
                {
                    span.InnerHtml += value;
                }
                else
                {
                    TagBuilder suffix = new TagBuilder("span");
                    suffix.AddCssClass("rate-suffix");
                    suffix.SetInnerText(mixedValue[1]);
                    span.InnerHtml += mixedValue[0] + "&nbsp;" + suffix.ToString(TagRenderMode.Normal);
                }
            }
            else
            {
                span.SetInnerText("-");
            }

            return new MvcHtmlString(span.ToString(TagRenderMode.Normal));
        }


        public static MvcHtmlString FormattedBytesFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, string value)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);

            return helper.FormattedBytes(value);
        }
    }
}
