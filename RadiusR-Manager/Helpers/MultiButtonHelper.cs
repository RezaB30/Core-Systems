using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Helpers
{
    public static class MultiButtonHelper
    {
        public static MvcHtmlString MultiButton<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, object itemId, string action, string controller)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var selectedValue = Convert.ToInt32(metadata.Model);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var noPrefixFieldName = (fullName.Contains('.')) ? fullName.Substring(fullName.LastIndexOf('.') + 1) : fullName;

            var formUrl = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection).Action(action, controller);

            object _enumType;
            object _resourceType;
            if (metadata.AdditionalValues.TryGetValue("EnumType", out _enumType) && metadata.AdditionalValues.TryGetValue("EnumResourceType", out _resourceType))
            {
                var enumType = _enumType as Type;
                var resourceType = _resourceType as Type;
                var type = typeof(LocalizedList<,>).MakeGenericType(enumType, resourceType);
                var genericList = Activator.CreateInstance(type) as LocalizedList;
                var pairList = genericList.GetList();

                TagBuilder wrapper = new TagBuilder("div");
                wrapper.MergeAttribute("tabindex", "0");
                wrapper.AddCssClass("multi-button-wrapper");

                TagBuilder mainButton = new TagBuilder("div");
                mainButton.AddCssClass("multi-button-item-top");
                mainButton.SetInnerText(pairList.FirstOrDefault(p => p.Key == selectedValue).Value);
                mainButton.MergeAttribute("data-value", selectedValue.ToString());
                wrapper.InnerHtml += mainButton.ToString(TagRenderMode.Normal);

                TagBuilder optionsWrapper = new TagBuilder("div");
                optionsWrapper.AddCssClass("multi-button-options");

                pairList = pairList.Where(pl => pl.Key != selectedValue).ToDictionary(pl => pl.Key, pl => pl.Value);
                foreach (var item in pairList)
                {
                    TagBuilder optionButton = new TagBuilder("div");
                    optionButton.AddCssClass("multi-button-item");
                    optionButton.SetInnerText(item.Value);
                    optionButton.MergeAttribute("data-value", item.Key.ToString());
                    optionsWrapper.InnerHtml += optionButton.ToString(TagRenderMode.Normal);
                }

                wrapper.InnerHtml += optionsWrapper.ToString(TagRenderMode.Normal);

                TagBuilder form = new TagBuilder("form");
                form.MergeAttribute("confirm", "enabled");
                form.MergeAttribute("action", formUrl);
                form.MergeAttribute("method", "post");
                form.MergeAttribute("style", "display: none;");

                form.InnerHtml += helper.AntiForgeryToken();

                TagBuilder hidden = new TagBuilder("input");
                hidden.MergeAttribute("type", "hidden");
                hidden.MergeAttribute("name", noPrefixFieldName);
                hidden.MergeAttribute("value", selectedValue.ToString());
                form.InnerHtml += hidden.ToString(TagRenderMode.SelfClosing);

                TagBuilder idHidden = new TagBuilder("input");
                idHidden.MergeAttribute("type", "hidden");
                idHidden.MergeAttribute("name", "id");
                idHidden.MergeAttribute("value", itemId.ToString());
                form.InnerHtml += idHidden.ToString();

                TagBuilder urlHidden = new TagBuilder("input");
                urlHidden.MergeAttribute("type", "hidden");
                urlHidden.MergeAttribute("name", "redirectUrl");
                urlHidden.MergeAttribute("value", helper.ViewContext.HttpContext.Request.Url.AbsoluteUri);
                form.InnerHtml += urlHidden.ToString();

                wrapper.InnerHtml += form.ToString(TagRenderMode.Normal);

                return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString MultiButton<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, object itemId, string action)
        {
            return helper.MultiButton(expression, itemId, action, null);
        }

        public static MvcHtmlString MultiButton<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, object itemId)
        {
            return helper.MultiButton(expression, itemId, null, null);
        }
    }
}