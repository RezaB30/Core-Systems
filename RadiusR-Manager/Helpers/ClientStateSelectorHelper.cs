using RadiusR.DB.Enums;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Helpers
{
    public static class ClientStateSelectorHelper
    {
        public static MvcHtmlString ClientStateSelector<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, long id, CustomerState selected, string action, string controller = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var selectedValue = selected;
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var noPrefixFieldName = (fullName.Contains('.')) ? fullName.Substring(fullName.LastIndexOf('.') + 1) : fullName;
            var itemsList = metadata.Model as IEnumerable<CustomerState>;
            var localizedList = new LocalizedList<CustomerState, RadiusR.Localization.Lists.CustomerState>();

            var formUrl = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection).Action(action, controller);

            if (itemsList != null)
            {
                TagBuilder wrapper = new TagBuilder("div");
                wrapper.MergeAttribute("tabindex", "0");
                wrapper.AddCssClass("multi-button-wrapper");
                wrapper.AddCssClass("client-state-selector-wrapper");

                TagBuilder mainButton = new TagBuilder("div");
                mainButton.AddCssClass("multi-button-item-top");
                mainButton.MergeAttribute("data-value", ((short)selectedValue).ToString());

                {
                    TagBuilder bullet = new TagBuilder("div");
                    bullet.AddCssClass("client-state-selector-bullet");
                    bullet.AddCssClass("customer-state-container-" + (short)selected);

                    mainButton.InnerHtml += bullet.ToString(TagRenderMode.Normal);
                    mainButton.InnerHtml += localizedList.GetDisplayText((short)selected);
                }
                wrapper.InnerHtml += mainButton.ToString(TagRenderMode.Normal);

                TagBuilder optionsWrapper = new TagBuilder("div");
                optionsWrapper.AddCssClass("multi-button-options");

                foreach (var item in itemsList)
                {
                    TagBuilder optionButton = new TagBuilder("div");
                    optionButton.AddCssClass("multi-button-item");
                    optionButton.MergeAttribute("data-value", ((short)item).ToString());

                    {
                        TagBuilder bullet = new TagBuilder("div");
                        bullet.AddCssClass("client-state-selector-bullet");
                        bullet.AddCssClass("customer-state-container-" + (short)item);

                        optionButton.InnerHtml += bullet.ToString(TagRenderMode.Normal);
                        optionButton.InnerHtml += localizedList.GetDisplayText((short)item);
                    }

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
                hidden.MergeAttribute("name", "State");
                hidden.MergeAttribute("value", ((short)selectedValue).ToString());
                form.InnerHtml += hidden.ToString(TagRenderMode.SelfClosing);

                TagBuilder idHidden = new TagBuilder("input");
                idHidden.MergeAttribute("type", "hidden");
                idHidden.MergeAttribute("name", "id");
                idHidden.MergeAttribute("value", id.ToString());
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