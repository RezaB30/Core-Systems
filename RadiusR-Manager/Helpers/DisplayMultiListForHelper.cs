using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR_Manager.Helpers
{
    public static class DisplayMultiListForHelper
    {
        public static MvcHtmlString DisplayMultiListFor<TModel, TResult, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, IEnumerable<TResult>>> expression, Expression<Func<TResult, TValue>> memberExpression, object htmlAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var values = metadata.Model as IEnumerable<TResult>;

            TagBuilder wrapper = new TagBuilder("span");
            wrapper.MergeAttribute("style", "white-space: pre-line;");

            var displayList = new List<MvcHtmlString>();
            foreach (var value in values)
            {
                var memberName = (memberExpression.Body as MemberExpression).Member.Name;
                var parameterExpression = Expression.Constant(value);
                var displayExpression = Expression.Property(parameterExpression, memberName);
                var finalExpression = Expression.Lambda<Func<TModel, TValue>>(displayExpression, new ParameterExpression[] { Expression.Parameter(typeof(TModel), "modelItem") });

                displayList.Add(helper.DisplayFor(finalExpression));
            }

            wrapper.InnerHtml += string.Join(Environment.NewLine, displayList);

            if (htmlAttributes != null)
            {
                foreach (var propertyName in htmlAttributes.GetType().GetProperties().Select(property => property.Name))
                {
                    wrapper.MergeAttribute(propertyName, htmlAttributes.GetType().GetProperty(propertyName).GetValue(htmlAttributes).ToString());
                }
            }

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}