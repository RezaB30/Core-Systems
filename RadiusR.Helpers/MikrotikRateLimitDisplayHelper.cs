using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR.Helpers
{
    public static class MikrotikRateLimitDisplayHelper
    {
        public static MvcHtmlString MikrotikRateLimitDisplayFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : MikrotikRateLimitViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var Model = (MikrotikRateLimitViewModel)metadata.Model;
            var convertDictionary = new Dictionary<string, string>()
            {
                { "", "bps" },
                { "k", "Kbps" },
                { "M", "Mbps" }
            };

            if (Model == null)
            {
                return new MvcHtmlString("<span class='text-danger'>" + RadiusR.Localization.Helpers.Common.InvalidRateLimit + "</span>");
            }

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
                    label.InnerHtml = helper.DisplayNameFor(model => Model.TxView).ToHtmlString() + ":";
                    suffix.SetInnerText(convertDictionary[Model.TxSuffix]);

                    cell.InnerHtml += label.ToString(TagRenderMode.Normal) + Model.TxView + suffix.ToString(TagRenderMode.Normal);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    TagBuilder suffix = new TagBuilder("span");
                    suffix.AddCssClass("rate-suffix");
                    TagBuilder label = new TagBuilder("span");
                    label.AddCssClass("rate-limit-label");
                    label.InnerHtml = helper.DisplayNameFor(model => Model.RxView).ToHtmlString() + ":";
                    suffix.SetInnerText(convertDictionary[Model.RxSuffix]);

                    cell.InnerHtml += label.ToString(TagRenderMode.Normal) + Model.RxView + suffix.ToString(TagRenderMode.Normal);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            if (Model.TxBurst.HasValue || Model.RxBurst.HasValue)
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    TagBuilder suffix = new TagBuilder("span");
                    suffix.AddCssClass("rate-suffix");
                    TagBuilder label = new TagBuilder("span");
                    label.AddCssClass("rate-limit-label");
                    label.InnerHtml = helper.DisplayNameFor(model => Model.TxBurstView).ToHtmlString() + ":";
                    suffix.SetInnerText(convertDictionary[Model.TxBurstSuffix]);

                    cell.InnerHtml += label.ToString(TagRenderMode.Normal) + Model.TxBurstView + suffix.ToString(TagRenderMode.Normal);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    TagBuilder suffix = new TagBuilder("span");
                    suffix.AddCssClass("rate-suffix");
                    TagBuilder label = new TagBuilder("span");
                    label.AddCssClass("rate-limit-label");
                    label.InnerHtml = helper.DisplayNameFor(model => Model.RxBurstView).ToHtmlString() + ":";
                    suffix.SetInnerText(convertDictionary[Model.RxBurstSuffix]);

                    cell.InnerHtml += label.ToString(TagRenderMode.Normal) + Model.RxBurstView + suffix.ToString(TagRenderMode.Normal);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            if (Model.TxBurstThreshold.HasValue || Model.RxBurstThreshold.HasValue)
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    TagBuilder suffix = new TagBuilder("span");
                    suffix.AddCssClass("rate-suffix");
                    TagBuilder label = new TagBuilder("span");
                    label.AddCssClass("rate-limit-label");
                    label.InnerHtml = helper.DisplayNameFor(model => Model.TxBurstThresholdView).ToHtmlString() + ":";
                    suffix.SetInnerText(convertDictionary[Model.TxBurstThresholdSuffix]);

                    cell.InnerHtml += label.ToString(TagRenderMode.Normal) + Model.TxBurstThresholdView + suffix.ToString(TagRenderMode.Normal);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    TagBuilder suffix = new TagBuilder("span");
                    suffix.AddCssClass("rate-suffix");
                    TagBuilder label = new TagBuilder("span");
                    label.AddCssClass("rate-limit-label");
                    label.InnerHtml = helper.DisplayNameFor(model => Model.RxBurstThresholdView).ToHtmlString() + ":";
                    suffix.SetInnerText(convertDictionary[Model.RxBurstThresholdSuffix]);

                    cell.InnerHtml += label.ToString(TagRenderMode.Normal) + Model.RxBurstThresholdView + suffix.ToString(TagRenderMode.Normal);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            if (Model.TxBurstTime.HasValue || Model.RxBurstTime.HasValue)
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    TagBuilder label = new TagBuilder("span");
                    label.AddCssClass("rate-limit-label");
                    label.InnerHtml = helper.DisplayNameFor(model => Model.TxBurstTimeView).ToHtmlString() + ":";

                    cell.InnerHtml += label.ToString(TagRenderMode.Normal) + Model.TxBurstTimeView;
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    TagBuilder label = new TagBuilder("span");
                    label.AddCssClass("rate-limit-label");
                    label.InnerHtml = helper.DisplayNameFor(model => Model.RxBurstTimeView).ToHtmlString() + ":";

                    cell.InnerHtml += label.ToString(TagRenderMode.Normal) + Model.RxBurstTimeView;
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            if (Model.Priority.HasValue)
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    TagBuilder label = new TagBuilder("span");
                    label.AddCssClass("rate-limit-label");
                    label.InnerHtml = helper.DisplayNameFor(model => Model.Priority).ToHtmlString() + ":";

                    cell.InnerHtml += label.ToString(TagRenderMode.Normal) + Model.Priority;
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            return new MvcHtmlString(table.ToString(TagRenderMode.Normal));
        }
    }
}
