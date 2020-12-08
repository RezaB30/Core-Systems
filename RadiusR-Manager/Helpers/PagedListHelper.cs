using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Helpers
{
    public static class PagedListHelper
    {
        /// <summary>
        /// Makes a paging row for data tables
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="pageCount">Total number of pages in current table</param>
        /// <param name="pageNumber">Current data page</param>
        /// <returns></returns>
        public static MvcHtmlString PagedList(this HtmlHelper helper, int? pageCount = null, int? pageNumber = null)
        {
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            //retrieve page number from query string
            int routePage;
            if (!pageNumber.HasValue)
            {
                try
                {
                    routePage = Convert.ToInt32(HttpContext.Current.Request.QueryString.Get("page"));
                }
                catch (Exception)
                {
                    routePage = 0;
                }
                pageNumber = routePage;
            }
            // retrieve page count from view data
            if (!pageCount.HasValue)
            {
                try
                {
                    pageCount = helper.ViewBag.PageCount as int? ?? 1;
                }
                catch (Exception)
                {
                    pageCount = 1;
                }
            }
            //Makes a div wrapper around the entire element
            var wrapper = new TagBuilder("div");
            wrapper.AddCssClass("table-pages");
            //Make summary of pages
            var pagesSummary = new TagBuilder("span");
            pagesSummary.InnerHtml = string.Format(RadiusR.Localization.Helpers.Common.PageSummary, pageNumber + 1, pageCount, helper.ViewBag.PageTotalCount);
            wrapper.InnerHtml += pagesSummary.ToString(TagRenderMode.Normal);
            //Make first, previous and ... elements
            TagBuilder linksContainer = new TagBuilder("div");
            linksContainer.AddCssClass("page-numbers-container");

            var firstLink = new TagBuilder("a");
            firstLink.AddCssClass("page-link");
            firstLink.MergeAttribute("title", RadiusR.Localization.Helpers.Common.First);
            firstLink.MergeAttribute("href", urlHelper.Current(new { page = 0 }, new string[] { "errorMessage" }).ToString());
            if (pageNumber == 0)
            {
                firstLink.AddCssClass("disabled");
                firstLink.MergeAttribute("href", "javascript:void(0);", true);
            }
            firstLink.InnerHtml = "<<";
            var prevLink = new TagBuilder("a");
            prevLink.AddCssClass("page-link");
            prevLink.MergeAttribute("title", RadiusR.Localization.Helpers.Common.Previous);
            prevLink.MergeAttribute("href", urlHelper.Current(new { page = pageNumber - 1 }, new string[] { "errorMessage" }).ToString());
            if (pageNumber - 1 < 0)
            {
                prevLink.AddCssClass("disabled");
                prevLink.MergeAttribute("href", "javascript:void(0);", true);
            }
            prevLink.InnerHtml = "<";
            linksContainer.InnerHtml += firstLink.ToString(TagRenderMode.Normal)
                + prevLink.ToString(TagRenderMode.Normal);
            if (pageNumber - AppSettings.PagesLinkCount > 0)
            {
                linksContainer.InnerHtml += "...";
            }
            //Making page number links
            for (int i = pageNumber.Value - AppSettings.PagesLinkCount; i <= pageNumber.Value + AppSettings.PagesLinkCount; i++)
            {
                if (i >= 0 && i < pageCount)
                {
                    var pageLink = new TagBuilder("a");
                    pageLink.AddCssClass("page-link");
                    pageLink.MergeAttribute("href", urlHelper.Current(new { page = i }, new string[] { "errorMessage" }).ToString());
                    pageLink.InnerHtml = i + 1 + "";

                    if (i == pageNumber.Value)
                    {
                        pageLink.AddCssClass("disabled");
                        pageLink.MergeAttribute("href", "javascript:void(0);", true);
                    }

                    linksContainer.InnerHtml += pageLink.ToString(TagRenderMode.Normal);
                }
            }
            //Make next, last and ... elements
            if (pageNumber + AppSettings.PagesLinkCount < pageCount - 1)
            {
                linksContainer.InnerHtml += "...";
            }
            var nextLink = new TagBuilder("a");
            nextLink.AddCssClass("page-link");
            nextLink.MergeAttribute("title", RadiusR.Localization.Helpers.Common.Next);
            nextLink.MergeAttribute("href", urlHelper.Current(new { page = pageNumber + 1 }, new string[] { "errorMessage" }).ToString());
            if (pageNumber + 1 >= pageCount)
            {
                nextLink.AddCssClass("disabled");
                nextLink.MergeAttribute("href", "javascript:void(0);", true);
            }
            nextLink.InnerHtml = ">";
            var lastLink = new TagBuilder("a");
            lastLink.AddCssClass("page-link");
            lastLink.MergeAttribute("title", RadiusR.Localization.Helpers.Common.Last);
            lastLink.MergeAttribute("href", urlHelper.Current(new { page = pageCount - 1 }, new string[] { "errorMessage" }).ToString());
            if (pageNumber + 1 >= pageCount)
            {
                lastLink.AddCssClass("disabled");
                lastLink.MergeAttribute("href", "javascript:void(0);", true);
            }
            lastLink.InnerHtml = ">>";

            linksContainer.InnerHtml += nextLink.ToString(TagRenderMode.Normal)
                + lastLink.ToString(TagRenderMode.Normal);

            wrapper.InnerHtml += linksContainer.ToString(TagRenderMode.Normal);

            //return the result
            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}