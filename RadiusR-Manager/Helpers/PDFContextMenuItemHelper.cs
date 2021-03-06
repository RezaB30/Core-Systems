using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR_Manager.Helpers
{
    public static class PDFContextMenuItemHelper
    {
        public static MvcHtmlString PDFContextMenuItemFor<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, int>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var Model = (int)metadata.Model;

            var menuItem = new TagBuilder("div");
            menuItem.AddCssClass("pdf-context-menu-item");
            menuItem.MergeAttribute("content-id", Convert.ToString(Model));

            var menuItemLabel = new TagBuilder("span");
            menuItemLabel.SetInnerText(metadata.DisplayName);
            menuItem.InnerHtml = menuItemLabel.ToString(TagRenderMode.Normal);

            return new MvcHtmlString(menuItem.ToString(TagRenderMode.Normal));
        }
    }
}