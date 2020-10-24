using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using RezaB.Data.Formating;

namespace RadiusR.Helpers
{
    public static class TrafficLimitTextHelper
    {
        public static MvcHtmlString TrafficLimitText(this HtmlHelper helper, string value = null)
        {
            TagBuilder span = new TagBuilder("span");
            if (!string.IsNullOrEmpty(value))
            {
                decimal parsed;
                if (!decimal.TryParse(value, out parsed))
                {
                    span.SetInnerText("NaN");
                }
                else
                {
                    var mixedValue = RateLimitFormatter.ToTrafficMixedResults(parsed);
                    TagBuilder suffix = new TagBuilder("span");
                    suffix.AddCssClass("rate-suffix");
                    suffix.SetInnerText(mixedValue.Suffix);
                    span.InnerHtml += mixedValue.FieldValue + "&nbsp;" + suffix.ToString(TagRenderMode.Normal);
                }
            }
            else
            {
                span.SetInnerText("-");
            }

            return new MvcHtmlString(span.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString TrafficLimitText<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);

            return TrafficLimitText(helper, Convert.ToString(metadata.Model));
        }
    }
}