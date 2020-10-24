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
    public static class ClientStateDisplayHelper
    {
        public static MvcHtmlString ClientStateDisplay<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var selectedValue = (short)metadata.Model;
            var localizedList = new LocalizedList<CustomerState, RadiusR.Localization.Lists.CustomerState>();

            if (selectedValue != 0)
            {
                TagBuilder wrapper = new TagBuilder("div");
                wrapper.MergeAttribute("style", "display: inline-block; text-align: left;");

                TagBuilder mainButton = new TagBuilder("div");
                mainButton.AddCssClass("multi-button-item-top");
                mainButton.MergeAttribute("style", "background-image: none;");

                {
                    TagBuilder bullet = new TagBuilder("div");
                    bullet.AddCssClass("client-state-selector-bullet");
                    bullet.AddCssClass("customer-state-container-" + selectedValue);

                    mainButton.InnerHtml += bullet.ToString(TagRenderMode.Normal);
                    mainButton.InnerHtml += localizedList.GetDisplayText(selectedValue);
                }
                wrapper.InnerHtml += mainButton.ToString(TagRenderMode.Normal);

                return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
            }
            return MvcHtmlString.Empty;
        }
    }
}