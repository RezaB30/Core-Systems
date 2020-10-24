using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR_Manager.Helpers
{
    public static class SMSParameterEditorHelper
    {
        public static MvcHtmlString SMSParameterEditorFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, object htmlAttributes) where TModel : IEnumerable<SMSParameterViewModel>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var modelValue = metadata.Model as IEnumerable<SMSParameterViewModel>;

            TagBuilder openButton = new TagBuilder("input");
            openButton.MergeAttribute("type", "button");
            openButton.AddCssClass("link-button iconed-button next-button sms-parameter-open-button");

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("sms-parameter-wrapper");
            wrapper.AddCssClass("hidden");

            foreach (var item in modelValue)
            {
                TagBuilder SMSParameterItem = new TagBuilder("div");
                SMSParameterItem.AddCssClass("sms-parameter-item");
                SMSParameterItem.MergeAttribute("data-value", item.Name);
                SMSParameterItem.MergeAttribute("tabindex", "0");
                SMSParameterItem.InnerHtml = helper.DisplayFor(modelItem => item.DisplayName).ToHtmlString();

                TagBuilder SMSParameterName = new TagBuilder("span");
                SMSParameterName.AddCssClass("sms-parameter-name");
                SMSParameterName.InnerHtml = helper.DisplayFor(modelItem => item.Name).ToHtmlString();

                SMSParameterItem.InnerHtml += SMSParameterName.ToString(TagRenderMode.Normal);
                wrapper.InnerHtml += SMSParameterItem.ToString(TagRenderMode.Normal);
            }

            if (htmlAttributes != null)
            {
                foreach (var propertyName in htmlAttributes.GetType().GetProperties().Select(property => property.Name))
                {
                    wrapper.MergeAttribute(propertyName, htmlAttributes.GetType().GetProperty(propertyName).GetValue(htmlAttributes).ToString());
                }
            }

            return new MvcHtmlString(openButton.ToString(TagRenderMode.SelfClosing) + wrapper.ToString(TagRenderMode.Normal));
        }
    }
}