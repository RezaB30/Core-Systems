using RadiusR_Manager.Models.RadiusViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RadiusR.Helpers
{
    public static class AddressSearchHelper
    {
        public static MvcHtmlString AddressSearchFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : AddressViewModel
        {
            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("address-search-wrapper");

            TagBuilder addressText = new TagBuilder("div");
            addressText.AddCssClass("address-text");
            wrapper.InnerHtml += addressText.ToString(TagRenderMode.Normal);

            TagBuilder selectButton = new TagBuilder("input");
            selectButton.MergeAttribute("type", "button");
            selectButton.AddCssClass("link-button iconed-button accept-button open-list-button");
            selectButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.Select);
            wrapper.InnerHtml += selectButton.ToString(TagRenderMode.SelfClosing);

            TagBuilder addressInputWrapper = new TagBuilder("div");
            addressInputWrapper.MergeAttribute("style", "display: none;");
            addressInputWrapper.AddCssClass("address-input-wrapper");
            addressInputWrapper.InnerHtml = helper.AddressEditorFor(expression).ToHtmlString();

            TagBuilder acceptRow = new TagBuilder("div");
            acceptRow.MergeAttribute("style", "text-align: right;");
            TagBuilder acceptButton = new TagBuilder("input");
            acceptButton.MergeAttribute("type", "button");
            acceptButton.AddCssClass("link-button iconed-button accept-button close-list-button");
            acceptButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.OK);
            acceptRow.InnerHtml = acceptButton.ToString(TagRenderMode.SelfClosing);
            addressInputWrapper.InnerHtml += acceptRow.ToString(TagRenderMode.Normal);

            wrapper.InnerHtml += addressInputWrapper.ToString(TagRenderMode.Normal);
            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}
