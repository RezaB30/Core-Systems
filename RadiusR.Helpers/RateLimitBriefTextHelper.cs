using RezaB.Data.Formating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace RadiusR.Helpers
{
    public static class RateLimitBriefTextHelper
    {
        public static MvcHtmlString BriefRateLimitText(this HtmlHelper helper, string fieldName, string value = null)
        {
            RateLimitParser.RateLimitAttributeList limitAttributes = null;
            if (value != null)
                limitAttributes = RateLimitParser.ParseString(value);
            else
                return new MvcHtmlString("-");

            TagBuilder table = new TagBuilder("table");
            table.AddCssClass("rate-limit-text");
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    TagBuilder suffix = new TagBuilder("span");
                    suffix.AddCssClass("rate-suffix");
                    TagBuilder label = new TagBuilder("span");
                    label.AddCssClass("rate-limit-label");
                    label.SetInnerText(RadiusR.Localization.Model.FreeRadius.DownloadRate + ": ");
                    suffix.SetInnerText(GetNormalizedSuffix(limitAttributes.DownloadRateSuffix));

                    var stringValue = limitAttributes.DownloadRate.ToString();

                    cell.InnerHtml += label.ToString(TagRenderMode.Normal) + stringValue + suffix.ToString(TagRenderMode.Normal);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    TagBuilder suffix = new TagBuilder("span");
                    suffix.AddCssClass("rate-suffix");
                    TagBuilder label = new TagBuilder("span");
                    label.AddCssClass("rate-limit-label");
                    label.SetInnerText(RadiusR.Localization.Model.FreeRadius.UploadRate + ": ");
                    suffix.SetInnerText(GetNormalizedSuffix(limitAttributes.UploadRateSuffix));

                    var stringValue = limitAttributes.UploadRate.ToString();

                    cell.InnerHtml += label.ToString(TagRenderMode.Normal) + stringValue + suffix.ToString(TagRenderMode.Normal);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            //{
            //    TagBuilder row = new TagBuilder("tr");
            //    {
            //        TagBuilder cell = new TagBuilder("td");
            //        TagBuilder suffix = new TagBuilder("span");
            //        suffix.AddCssClass("rate-suffix");
            //        TagBuilder label = new TagBuilder("span");
            //        label.AddCssClass("rate-limit-label");
            //        label.SetInnerText(RadiusR.Localization.Model.FreeRadius.DownloadBurstRate + ": ");
            //        suffix.SetInnerText(GetNormalizedSuffix(limitAttributes.DownloadBurstRateSuffix));

            //        var stringValue = limitAttributes.DownloadBurstRate.ToString();
            //        if (limitAttributes.DownloadBurstRate == null)
            //        {
            //            suffix.SetInnerText("");
            //            stringValue = "-";
            //        }

            //        cell.InnerHtml += label.ToString(TagRenderMode.Normal) + stringValue + suffix.ToString(TagRenderMode.Normal);
            //        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
            //    }
            //    {
            //        TagBuilder cell = new TagBuilder("td");
            //        TagBuilder suffix = new TagBuilder("span");
            //        suffix.AddCssClass("rate-suffix");
            //        TagBuilder label = new TagBuilder("span");
            //        label.AddCssClass("rate-limit-label");
            //        label.SetInnerText(RadiusR.Localization.Model.FreeRadius.UploadBurstRate + ": ");
            //        suffix.SetInnerText(GetNormalizedSuffix(limitAttributes.UploadBurstRateSuffix));

            //        var stringValue = limitAttributes.UploadBurstRate.ToString();
            //        if (limitAttributes.UploadBurstRate == null)
            //        {
            //            suffix.SetInnerText("");
            //            stringValue = "-";
            //        }

            //        cell.InnerHtml += label.ToString(TagRenderMode.Normal) + stringValue + suffix.ToString(TagRenderMode.Normal);
            //        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
            //    }
            //    table.InnerHtml += row.ToString(TagRenderMode.Normal);
            //}
            //{
            //    TagBuilder row = new TagBuilder("tr");
            //    {
            //        TagBuilder cell = new TagBuilder("td");
            //        TagBuilder suffix = new TagBuilder("span");
            //        suffix.AddCssClass("rate-suffix");
            //        TagBuilder label = new TagBuilder("span");
            //        label.AddCssClass("rate-limit-label");
            //        label.SetInnerText(RadiusR.Localization.Model.FreeRadius.DownloadBurstThreshold + ": ");
            //        suffix.SetInnerText(GetNormalizedSuffix(limitAttributes.DownloadBurstThresholdSuffix));

            //        var stringValue = limitAttributes.DownloadBurstThreshold.ToString();
            //        if (limitAttributes.DownloadBurstThreshold == null)
            //        {
            //            suffix.SetInnerText("");
            //            stringValue = "-";
            //        }

            //        cell.InnerHtml += label.ToString(TagRenderMode.Normal) + stringValue + suffix.ToString(TagRenderMode.Normal);
            //        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
            //    }
            //    {
            //        TagBuilder cell = new TagBuilder("td");
            //        TagBuilder suffix = new TagBuilder("span");
            //        suffix.AddCssClass("rate-suffix");
            //        TagBuilder label = new TagBuilder("span");
            //        label.AddCssClass("rate-limit-label");
            //        label.SetInnerText(RadiusR.Localization.Model.FreeRadius.UploadBurstThreshold + ": ");
            //        suffix.SetInnerText(GetNormalizedSuffix(limitAttributes.UploadBurstThresholdSuffix));

            //        var stringValue = limitAttributes.UploadBurstThreshold.ToString();
            //        if (limitAttributes.UploadBurstThreshold == null)
            //        {
            //            suffix.SetInnerText("");
            //            stringValue = "-";
            //        }

            //        cell.InnerHtml += label.ToString(TagRenderMode.Normal) + stringValue + suffix.ToString(TagRenderMode.Normal);
            //        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
            //    }
            //    table.InnerHtml += row.ToString(TagRenderMode.Normal);
            //}
            //{
            //    TagBuilder row = new TagBuilder("tr");
            //    {
            //        TagBuilder cell = new TagBuilder("td");
            //        TagBuilder suffix = new TagBuilder("span");
            //        suffix.AddCssClass("rate-suffix");
            //        TagBuilder label = new TagBuilder("span");
            //        label.AddCssClass("rate-limit-label");
            //        label.SetInnerText(RadiusR.Localization.Model.FreeRadius.DownloadBurstTime + ": ");
            //        suffix.SetInnerText("s");

            //        var stringValue = limitAttributes.DownloadBurstTime.ToString();
            //        if (limitAttributes.DownloadBurstTime == null)
            //        {
            //            suffix.SetInnerText("");
            //            stringValue = "-";
            //        }

            //        cell.InnerHtml += label.ToString(TagRenderMode.Normal) + stringValue + suffix.ToString(TagRenderMode.Normal);
            //        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
            //    }
            //    {
            //        TagBuilder cell = new TagBuilder("td");
            //        TagBuilder suffix = new TagBuilder("span");
            //        suffix.AddCssClass("rate-suffix");
            //        TagBuilder label = new TagBuilder("span");
            //        label.AddCssClass("rate-limit-label");
            //        label.SetInnerText(RadiusR.Localization.Model.FreeRadius.UploadBurstTime + ": ");
            //        suffix.SetInnerText("s");

            //        var stringValue = limitAttributes.UploadBurstTime.ToString();
            //        if (limitAttributes.UploadBurstTime == null)
            //        {
            //            suffix.SetInnerText("");
            //            stringValue = "-";
            //        }

            //        cell.InnerHtml += label.ToString(TagRenderMode.Normal) + stringValue + suffix.ToString(TagRenderMode.Normal);
            //        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
            //    }
            //    table.InnerHtml += row.ToString(TagRenderMode.Normal);
            //}
            return new MvcHtmlString(table.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString BriefRateLimitText<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, string value)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);

            return BriefRateLimitText(helper, fullName, value);
        }

        private static string GetNormalizedSuffix(string suffix)
        {
            switch (suffix)
            {
                case "k":
                    return "Kbps";
                case "M":
                    return "Mbps";
                default:
                    return "bps";
            }
        }
    }
}