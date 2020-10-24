using RadiusR_Manager.Models.ViewModels;
using RezaB.Web.Helpers;
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
    public static class MikrotikRateLimitEditorHelper
    {
        public static MvcHtmlString MikrotikRateLimitEditorFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : MikrotikRateLimitViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var Model = (MikrotikRateLimitViewModel)metadata.Model ?? new MikrotikRateLimitViewModel();
            var suffixList = new[]
            {
                new { Name = "bps", Value = string.Empty},
                new { Name = "Kbps", Value = "k"},
                new { Name = "Mbps", Value = "M"}
            };

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("mikrotik-rate-limit-wrapper");

            TagBuilder table = new TagBuilder("table");
            table.AddCssClass("input-table");

            // rate row
            {
                TagBuilder row = new TagBuilder("tr");
                // download rate
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.TxView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => Model.TxView, new { @htmlAttributes = new { @maxlength = 10, @autocomplete = "off", @class = "rate-input" } }).ToHtmlString();
                    cell.InnerHtml += helper.Select(model => Model.TxSuffix, new SelectList(suffixList, "Value", "Name", Model.TxSuffix), htmlAttributes: new { @class = "rate-select" }).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                // upload rate
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.RxView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => Model.RxView, new { @htmlAttributes = new { @maxlength = 10, @autocomplete = "off", @class = "rate-input" } }).ToHtmlString();
                    cell.InnerHtml += helper.Select(model => Model.RxSuffix, new SelectList(suffixList, "Value", "Name", Model.RxSuffix), htmlAttributes: new { @class = "rate-select" }).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            // rate validation row
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.TxView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.RxView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }

            // burst rate row
            {
                TagBuilder row = new TagBuilder("tr");
                // download rate
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.TxBurstView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => Model.TxBurstView, new { @htmlAttributes = new { @maxlength = 10, @autocomplete = "off", @class = "rate-input" } }).ToHtmlString();
                    cell.InnerHtml += helper.Select(model => Model.TxBurstSuffix, new SelectList(suffixList, "Value", "Name", Model.TxBurstSuffix), htmlAttributes: new { @class = "rate-select" }).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                // upload rate
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.RxBurstView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => Model.RxBurstView, new { @htmlAttributes = new { @maxlength = 10, @autocomplete = "off", @class = "rate-input" } }).ToHtmlString();
                    cell.InnerHtml += helper.Select(model => Model.RxBurstSuffix, new SelectList(suffixList, "Value", "Name", Model.RxBurstSuffix), htmlAttributes: new { @class = "rate-select" }).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            // burst rate validation row
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.TxBurstView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.RxBurstView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }

            // burst threshold rate row
            {
                TagBuilder row = new TagBuilder("tr");
                // download rate
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.TxBurstThresholdView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => Model.TxBurstThresholdView, new { @htmlAttributes = new { @maxlength = 10, @autocomplete = "off", @class = "rate-input" } }).ToHtmlString();
                    cell.InnerHtml += helper.Select(model => Model.TxBurstThresholdSuffix, new SelectList(suffixList, "Value", "Name", Model.TxBurstThresholdSuffix), htmlAttributes: new { @class = "rate-select" }).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                // upload rate
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.RxBurstThresholdView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => Model.RxBurstThresholdView, new { @htmlAttributes = new { @maxlength = 10, @autocomplete = "off", @class = "rate-input" } }).ToHtmlString();
                    cell.InnerHtml += helper.Select(model => Model.RxBurstThresholdSuffix, new SelectList(suffixList, "Value", "Name", Model.RxBurstThresholdSuffix), htmlAttributes: new { @class = "rate-select" }).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            // burst threshold rate validation row
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.TxBurstThresholdView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.RxBurstThresholdView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }

            // burst time row
            {
                TagBuilder row = new TagBuilder("tr");
                // download
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.TxBurstTimeView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => Model.TxBurstTimeView, new { @htmlAttributes = new { @maxlength = 10, @autocomplete = "off" } }).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                // upload
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.RxBurstTimeView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => Model.RxBurstTimeView, new { @htmlAttributes = new { @maxlength = 10, @autocomplete = "off"} }).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            // burst time validation row
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.TxBurstTimeView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.RxBurstTimeView).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            // priority row
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.Priority).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "3");
                    cell.InnerHtml = helper.Select(model => Model.Priority, HelperUtilities.CreateNumericSelectList(1, 8, Model.Priority), RadiusR.Localization.Helpers.Common.Select).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }

            wrapper.InnerHtml = table.ToString(TagRenderMode.Normal);
            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}
