using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR_Manager.Helpers
{
    public static class LanguageLinkHelper
    {
        /// <summary>
        /// Creates a link for changing language
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="linkText">link text</param>
        /// <param name="culture">culture name</param>
        /// <returns></returns>
        public static MvcHtmlString LanguageLink(this HtmlHelper helper, string linkText, string culture)
        {
            var currentRouteData = helper.ViewContext.RouteData.Values;
            HttpContext.Current.Request.QueryString.CopyTo(currentRouteData);
            object action;
            currentRouteData.TryGetValue("action", out action);
            currentRouteData["sender"] = action;
            currentRouteData["culture"] = culture;

            return helper.ActionLink(linkText, "Language", currentRouteData);
        }
    }
}