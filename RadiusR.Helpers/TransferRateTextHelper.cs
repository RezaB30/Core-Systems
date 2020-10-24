using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using RezaB.Data.Formating;

namespace RadiusR.Helpers
{
    public static class TransferRateTextHelper
    {
        public static MvcHtmlString TransferRateText<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var value = Convert.ToString(metadata.Model);

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
                    var mixedValue = RateLimitFormatter.ToTrafficMixedResults(parsed, true);
                    TagBuilder suffix = new TagBuilder("span");
                    suffix.AddCssClass("rate-suffix");
                    suffix.SetInnerText(mixedValue.RateSuffix);
                    span.InnerHtml += mixedValue.FieldValue + suffix.ToString(TagRenderMode.Normal);
                }
            }
            else
            {
                span.SetInnerText("-");
            }

            return new MvcHtmlString(span.ToString(TagRenderMode.Normal));
        }
    }
}
