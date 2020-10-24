using RezaB.Data.Formating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace RadiusR.Helpers
{
    public static class RateLimitHelper
    {
        public static MvcHtmlString RateLimit(this HtmlHelper helper, string fieldName, string value = null)
        {
            RateLimitParser.RateLimitAttributeList limitAttributes = null;
            if (value != null)
                limitAttributes = RateLimitParser.ParseString(value);

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("rate-limit-wrapper");

            TagBuilder hidden = new TagBuilder("input");
            hidden.MergeAttribute("id", fieldName);
            hidden.MergeAttribute("name", fieldName);
            hidden.MergeAttribute("type", "hidden");
            hidden.AddCssClass("rate-limit-hidden");
            if (!string.IsNullOrWhiteSpace(value))
                hidden.MergeAttribute("value", value);
            wrapper.InnerHtml += hidden.ToString(TagRenderMode.SelfClosing);

            TagBuilder table = new TagBuilder("table");
            table.AddCssClass("input-table");
            {
                TagBuilder row = new TagBuilder("tr");

                {
                    TagBuilder cell = new TagBuilder("td");

                    TagBuilder label = new TagBuilder("label");
                    label.MergeAttribute("for", fieldName + "_DownloadRate");
                    label.SetInnerText(RadiusR.Localization.Model.FreeRadius.DownloadRate);
                    cell.InnerHtml += label.ToString(TagRenderMode.Normal);

                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");

                    cell.InnerHtml += helper.TransferRate(/*fieldName + "_DownloadRate"*/null, (limitAttributes == null) ? (int?)null : limitAttributes.DownloadRate, (limitAttributes == null) ? null : limitAttributes.DownloadRateSuffix);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");

                    TagBuilder label = new TagBuilder("label");
                    label.MergeAttribute("for", fieldName + "_UploadRate");
                    label.SetInnerText(RadiusR.Localization.Model.FreeRadius.UploadRate);
                    cell.InnerHtml += label.ToString(TagRenderMode.Normal);

                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");

                    cell.InnerHtml += helper.TransferRate(/*fieldName + "_UploadRate"*/null, (limitAttributes == null) ? (int?)null : limitAttributes.UploadRate, (limitAttributes == null) ? null : limitAttributes.UploadRateSuffix);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }

                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");

                {
                    TagBuilder cell = new TagBuilder("td");

                    TagBuilder label = new TagBuilder("label");
                    label.MergeAttribute("for", fieldName + "_DownloadBurstRate");
                    label.SetInnerText(RadiusR.Localization.Model.FreeRadius.DownloadBurstRate);
                    cell.InnerHtml += label.ToString(TagRenderMode.Normal);

                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");

                    cell.InnerHtml += helper.TransferRate(/*fieldName + "_DownloadBurstRate"*/null, (limitAttributes == null) ? (int?)null : limitAttributes.DownloadBurstRate, (limitAttributes == null) ? null : limitAttributes.DownloadBurstRateSuffix);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");

                    TagBuilder label = new TagBuilder("label");
                    label.MergeAttribute("for", fieldName + "_UploadBurstRate");
                    label.SetInnerText(RadiusR.Localization.Model.FreeRadius.UploadBurstRate);
                    cell.InnerHtml += label.ToString(TagRenderMode.Normal);

                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");

                    cell.InnerHtml += helper.TransferRate(/*fieldName + "_UploadBurstRate"*/null, (limitAttributes == null) ? (int?)null : limitAttributes.UploadBurstRate, (limitAttributes == null) ? null : limitAttributes.UploadBurstRateSuffix);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }

                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");

                {
                    TagBuilder cell = new TagBuilder("td");

                    TagBuilder label = new TagBuilder("label");
                    label.MergeAttribute("for", fieldName + "_DownloadBurstThreshold");
                    label.SetInnerText(RadiusR.Localization.Model.FreeRadius.DownloadBurstThreshold);
                    cell.InnerHtml += label.ToString(TagRenderMode.Normal);

                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");

                    cell.InnerHtml += helper.TransferRate(/*fieldName + "_DownloadBurstThreshold"*/null, (limitAttributes == null) ? (int?)null : limitAttributes.DownloadBurstThreshold, (limitAttributes == null) ? null : limitAttributes.DownloadBurstThresholdSuffix);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");

                    TagBuilder label = new TagBuilder("label");
                    label.MergeAttribute("for", fieldName + "_UploadBurstThreshold");
                    label.SetInnerText(RadiusR.Localization.Model.FreeRadius.UploadBurstThreshold);
                    cell.InnerHtml += label.ToString(TagRenderMode.Normal);

                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");

                    cell.InnerHtml += helper.TransferRate(/*fieldName + "_UploadBurstThreshold"*/null, (limitAttributes == null) ? (int?)null : limitAttributes.UploadBurstThreshold, (limitAttributes == null) ? null : limitAttributes.UploadBurstThresholdSuffix);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }

                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");

                {
                    TagBuilder cell = new TagBuilder("td");

                    TagBuilder label = new TagBuilder("label");
                    label.MergeAttribute("for", fieldName + "_DownloadBurstTime");
                    label.SetInnerText(RadiusR.Localization.Model.FreeRadius.DownloadBurstTime);
                    cell.InnerHtml += label.ToString(TagRenderMode.Normal);

                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");

                    TagBuilder textbox = new TagBuilder("input");
                    textbox.GenerateId(fieldName + "_DownloadBurstTime");
                    textbox.MergeAttribute("type", "text");
                    textbox.MergeAttribute("value", (limitAttributes == null || !limitAttributes.DownloadBurstTime.HasValue) ? "" : limitAttributes.DownloadBurstTime.ToString());
                    textbox.MergeAttribute("autocomplete", "off");
                    textbox.AddCssClass("rate-limit-time");

                    cell.InnerHtml += textbox.ToString(TagRenderMode.SelfClosing);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");

                    TagBuilder label = new TagBuilder("label");
                    label.MergeAttribute("for", fieldName + "_UploadBurstTime");
                    label.SetInnerText(RadiusR.Localization.Model.FreeRadius.UploadBurstTime);
                    cell.InnerHtml += label.ToString(TagRenderMode.Normal);

                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");

                    TagBuilder textbox = new TagBuilder("input");
                    textbox.GenerateId(fieldName + "_UploadBurstTime");
                    textbox.MergeAttribute("type", "text");
                    textbox.MergeAttribute("value", (limitAttributes == null || !limitAttributes.UploadBurstTime.HasValue) ? "" : limitAttributes.UploadBurstTime.ToString());
                    textbox.MergeAttribute("autocomplete", "off");
                    textbox.AddCssClass("rate-limit-time");

                    cell.InnerHtml += textbox.ToString(TagRenderMode.SelfClosing);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }

                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            wrapper.InnerHtml += table.ToString(TagRenderMode.Normal);

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString RateLimit<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, string value)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);

            return RateLimit(helper, fullName, value);
        }
    }
}