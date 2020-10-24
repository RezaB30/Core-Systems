using RadiusR_Manager.Models.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR_Manager.Helpers
{
    public static class RelatedSubscriptionsHelper
    {
        public static MvcHtmlString RelatedSubscriptionsDisplay<TModel,TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel,TResult>> expression) where TResult : IEnumerable<RelatedSubscriptionsViewModel>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var value = metadata.Model as IEnumerable<RelatedSubscriptionsViewModel> ?? Enumerable.Empty<RelatedSubscriptionsViewModel>();

            var totalCount = value.Count();
            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("related-subscriptions-wrapper");
            wrapper.MergeAttribute("tabindex", "0");

            TagBuilder mainButton = new TagBuilder("div");
            mainButton.AddCssClass("main-button");
            mainButton.SetInnerText(string.Format(RadiusR.Localization.Helpers.Common.RelatedSubscriptions, totalCount));
            wrapper.InnerHtml += mainButton.ToString(TagRenderMode.Normal);

            TagBuilder listContainer = new TagBuilder("div");
            listContainer.AddCssClass("list-container");

            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            foreach (var item in value)
            {
                TagBuilder listLink = new TagBuilder("a");
                listLink.MergeAttribute("href", urlHelper.Action(null, null, new { id = item.ID }));

                TagBuilder listItem = new TagBuilder("div");
                listItem.AddCssClass("list-item");

                TagBuilder listBullet = new TagBuilder("div");
                listBullet.AddCssClass("client-state-selector-bullet customer-state-container-" + item.State);
                listBullet.MergeAttribute("title", helper.DisplayFor(modelItem => item.State).ToHtmlString().Trim());
                listItem.InnerHtml = listBullet.ToString(TagRenderMode.Normal);

                listItem.InnerHtml += helper.DisplayFor(modelItem => item.SubscriberNo);
                listLink.InnerHtml = listItem.ToString(TagRenderMode.Normal);
                listContainer.InnerHtml += listLink.ToString(TagRenderMode.Normal);
            }

            wrapper.InnerHtml += listContainer.ToString(TagRenderMode.Normal);

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}