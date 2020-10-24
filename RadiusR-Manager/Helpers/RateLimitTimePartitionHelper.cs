using RadiusR_Manager.Models.RadiusViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR_Manager.Helpers
{
    public static class RateLimitTimePartitionHelper
    {
        public static MvcHtmlString RateLimitTimePartitionEditorFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TModel : IEnumerable<ServiceRateTimePartitionViewModel>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = metadata.Model != null ? ((IEnumerable<ServiceRateTimePartitionViewModel>)metadata.Model).ToArray() : Enumerable.Empty<ServiceRateTimePartitionViewModel>().ToArray();
            var sample = new ServiceRateTimePartitionViewModel();

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("rate-limit-time-partition-wrapper");

            // sample row
            {
                TagBuilder rowItemContainer = new TagBuilder("div");
                rowItemContainer.AddCssClass("sample-row");
                // time selection
                {
                    TagBuilder timeSelection = new TagBuilder("div");
                    timeSelection.AddCssClass("time-selection");
                    TagBuilder timeSelectionTable = new TagBuilder("table");
                    timeSelectionTable.MergeAttribute("style", "width: 100%;");
                    {
                        TagBuilder row = new TagBuilder("tr");
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.MergeAttribute("style", "width: 25%");
                            cell.InnerHtml = helper.LabelFor(modelItem => sample.StartTime).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.MergeAttribute("style", "width: 25%");
                            cell.InnerHtml = helper.EditorFor(modelItem => sample.StartTime, new { @htmlAttributes = new { @autocomplete = "off" } }).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.MergeAttribute("style", "width: 25%");
                            cell.InnerHtml = helper.LabelFor(modelItem => sample.EndTime).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.MergeAttribute("style", "width: 25%");
                            cell.InnerHtml = helper.EditorFor(modelItem => sample.EndTime, new { @htmlAttributes = new { @autocomplete = "off" } }).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        timeSelectionTable.InnerHtml += row.ToString(TagRenderMode.Normal);
                    }
                    timeSelection.InnerHtml = timeSelectionTable.ToString(TagRenderMode.Normal);
                    rowItemContainer.InnerHtml += timeSelection.ToString(TagRenderMode.Normal);
                }
                // rate limit
                {
                    TagBuilder rateLimit = new TagBuilder("div");
                    rateLimit.InnerHtml = helper.EditorFor(modelItem => sample.RateLimitView).ToHtmlString();
                    rowItemContainer.InnerHtml += rateLimit.ToString(TagRenderMode.Normal);
                }
                // remove button
                {
                    TagBuilder removeRow = new TagBuilder("div");
                    removeRow.AddCssClass("remove-row");
                    TagBuilder removeButton = new TagBuilder("input");
                    removeButton.MergeAttribute("type", "button");
                    removeButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.Remove);
                    removeButton.AddCssClass("link-button iconed-button delete-button");
                    removeRow.InnerHtml = removeButton.ToString(TagRenderMode.SelfClosing);
                    rowItemContainer.InnerHtml += removeRow.ToString(TagRenderMode.Normal);
                }

                wrapper.InnerHtml += rowItemContainer.ToString(TagRenderMode.Normal);
            }

            // present values
            for (int i = 0; i < model.Count(); i++)
            {
                TagBuilder rowItemContainer = new TagBuilder("div");
                rowItemContainer.AddCssClass("item-row");
                // time selection
                {
                    TagBuilder timeSelection = new TagBuilder("div");
                    timeSelection.AddCssClass("time-selection");
                    TagBuilder timeSelectionTable = new TagBuilder("table");
                    timeSelectionTable.MergeAttribute("style", "width: 100%;");
                    {
                        TagBuilder row = new TagBuilder("tr");
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.MergeAttribute("style", "width: 25%");
                            cell.InnerHtml = helper.LabelFor(modelItem => model[i].StartTime).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.MergeAttribute("style", "width: 25%");
                            cell.InnerHtml = helper.EditorFor(modelItem => model[i].StartTime, new { @htmlAttributes = new { @autocomplete = "off" } }).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.MergeAttribute("style", "width: 25%");
                            cell.InnerHtml = helper.LabelFor(modelItem => model[i].EndTime).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.MergeAttribute("style", "width: 25%");
                            cell.InnerHtml = helper.EditorFor(modelItem => model[i].EndTime, new { @htmlAttributes = new { @autocomplete = "off" } }).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        timeSelectionTable.InnerHtml += row.ToString(TagRenderMode.Normal);
                    }
                    // validation
                    {
                        TagBuilder row = new TagBuilder("tr");
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.MergeAttribute("colspan", "2");
                            cell.InnerHtml = helper.ValidationMessageFor(modelItem => model[i].StartTime).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        {
                            TagBuilder cell = new TagBuilder("td");
                            cell.MergeAttribute("colspan", "2");
                            cell.InnerHtml = helper.ValidationMessageFor(modelItem => model[i].EndTime).ToHtmlString();
                            row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                        }
                        timeSelectionTable.InnerHtml += row.ToString(TagRenderMode.Normal);
                    }
                    timeSelection.InnerHtml = timeSelectionTable.ToString(TagRenderMode.Normal);
                    rowItemContainer.InnerHtml += timeSelection.ToString(TagRenderMode.Normal);
                }
                // rate limit
                {
                    TagBuilder rateLimit = new TagBuilder("div");
                    rateLimit.InnerHtml = helper.EditorFor(modelItem => model[i].RateLimitView).ToHtmlString();
                    rowItemContainer.InnerHtml += rateLimit.ToString(TagRenderMode.Normal);
                    TagBuilder rateLimitValidation = new TagBuilder("div");
                    rateLimitValidation.AddCssClass("centered");
                    rateLimitValidation.InnerHtml = helper.ValidationMessageFor(modelItem => model[i].RateLimitView).ToHtmlString();
                    rowItemContainer.InnerHtml += rateLimitValidation.ToString(TagRenderMode.Normal);
                }
                // remove button
                {
                    TagBuilder removeRow = new TagBuilder("div");
                    removeRow.AddCssClass("remove-row");
                    TagBuilder removeButton = new TagBuilder("input");
                    removeButton.MergeAttribute("type", "button");
                    removeButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.Remove);
                    removeButton.AddCssClass("link-button iconed-button delete-button");
                    removeRow.InnerHtml = removeButton.ToString(TagRenderMode.SelfClosing);
                    rowItemContainer.InnerHtml += removeRow.ToString(TagRenderMode.Normal);
                }
                wrapper.InnerHtml += rowItemContainer.ToString(TagRenderMode.Normal);
            }

            // add button
            {
                TagBuilder rowItemContainer = new TagBuilder("div");
                rowItemContainer.AddCssClass("centered");
                rowItemContainer.AddCssClass("add-instance-row");
                TagBuilder addButton = new TagBuilder("input");
                addButton.MergeAttribute("type", "button");
                addButton.AddCssClass("link-button iconed-button add-instance-button");
                addButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.AddInstance);
                rowItemContainer.InnerHtml = addButton.ToString(TagRenderMode.SelfClosing);

                wrapper.InnerHtml += rowItemContainer.ToString(TagRenderMode.Normal);
            }

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}