using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR_Manager.Helpers
{
    public static class RecurringDiscountEditorHelper
    {
        public static MvcHtmlString RecurringDiscountEditorFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, SelectList FeeTypeList, bool showDescription = true) where TResult : RecurringDiscountViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var Model = metadata.Model as RecurringDiscountViewModel;

            helper.ViewData.TemplateInfo.HtmlFieldPrefix = fieldName;

            TagBuilder containerTable = new TagBuilder("table");
            containerTable.AddCssClass("input-table recurring-discount-editor");

            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.DiscountType).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => Model.DiscountType).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.ApplicationType).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => Model.ApplicationType).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                containerTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.DiscountType).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.ApplicationType).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                containerTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("discount-amount-related");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.Amount).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => Model.Amount).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                containerTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("discount-amount-related");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.Amount).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                containerTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("discount-percentage-amount-related");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.PercentageAmount).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => Model.PercentageAmount).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                containerTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("discount-percentage-amount-related");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.PercentageAmount).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                containerTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("discount-fee-type-related");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.FeeTypeID).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.Select(model => Model.FeeTypeID, FeeTypeList, RadiusR.Localization.Pages.Common.Choose).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                containerTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("discount-fee-type-related");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.FeeTypeID).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                containerTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => Model.ApplicationTimes).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.Select(model => Model.ApplicationTimes, HelperUtilities.CreateNumericSelectList(1, 24, Model.ApplicationTimes), RadiusR.Localization.Pages.Common.Choose).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.EditorFor(model => Model.OnlyFullInvoice).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                containerTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.ApplicationTimes).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.OnlyFullInvoice).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                containerTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            if(showDescription)
            {
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.Description).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "3");
                        cell.InnerHtml = helper.TextAreaFor(model => Model.Description, new { @maxlength = 300, @autocomplete = "off" }).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    containerTable.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "4");
                        cell.InnerHtml = helper.ValidationMessageFor(model => Model.Description).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    containerTable.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
            }
            
            return new MvcHtmlString(containerTable.ToString(TagRenderMode.Normal));
        }
    }
}