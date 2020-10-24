using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RezaB.Web.Helpers;

namespace RadiusR_Manager.Helpers
{
    public static class CustomFeeListHelper
    {
        public static MvcHtmlString CustomFeeList<TModel, Tresult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, Tresult>> expression) where TModel : IEnumerable<CustomFeeViewModel>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = ((IEnumerable<CustomFeeViewModel>)metadata.Model).ToArray();

            StringBuilder rows = new StringBuilder("");
            // add model prefix
            {
                var nameContainer = new TagBuilder("input");
                nameContainer.MergeAttribute("type", "hidden");
                nameContainer.GenerateId("custom-fee-model-name");
                nameContainer.MergeAttribute("value", fullName);
                rows.Append(nameContainer.ToString(TagRenderMode.SelfClosing));
            }
            // creating sample row
            {
                var row = new TagBuilder("tr");
                var sampleModel = new CustomFeeViewModel();
                {
                    row.AddCssClass("custom-fee-top-row");
                    row.AddCssClass("custom-fee-sample-row");
                    row.AddCssClass("custom-fee-row");
                    // add title text
                    var cell = new TagBuilder("td");
                    cell.InnerHtml += helper.DisplayNameFor(modelItem => sampleModel.Title);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    // add title editor
                    cell = new TagBuilder("td");
                    cell.InnerHtml += helper.EditorFor(modelItem => sampleModel.Title, new { htmlAttributes = new { @autocomplete = "off" } });
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    // add installment text
                    cell = new TagBuilder("td");
                    cell.InnerHtml += helper.DisplayNameFor(modelItem => sampleModel.InstallmentCount);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    // add installment editor
                    cell = new TagBuilder("td");
                    cell.InnerHtml += helper.Select(modelItem => sampleModel.InstallmentCount, HelperUtilities.CreateNumericSelectList(1, 24, 1));
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    // finish the row
                    rows.Append(row.ToString(TagRenderMode.Normal));
                    row = new TagBuilder("tr");
                    row.AddCssClass("custom-fee-bottom-row");
                    row.AddCssClass("custom-fee-sample-row");
                    row.AddCssClass("custom-fee-row");
                    // add price text
                    cell = new TagBuilder("td");
                    cell.InnerHtml += helper.DisplayNameFor(modelItem => sampleModel.Price);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    // add price editor
                    cell = new TagBuilder("td");
                    cell.InnerHtml += helper.EditorFor(modelItem => sampleModel.Price);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    // add remove button
                    cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.AddCssClass("centered");
                    var removeButton = new TagBuilder("input");
                    removeButton.AddCssClass("link-button");
                    removeButton.AddCssClass("iconed-button");
                    removeButton.AddCssClass("remove-button");
                    removeButton.MergeAttribute("type", "button");
                    removeButton.MergeAttribute("value", RadiusR.Localization.Pages.Common.Remove);
                    cell.InnerHtml += removeButton.ToString(TagRenderMode.SelfClosing);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    // finish the row
                    rows.Append(row.ToString(TagRenderMode.Normal));
                }
            }
            // add instances
            for (int i = 0; i < model.Length; i++)
            {
                var row = new TagBuilder("tr");

                row.AddCssClass("custom-fee-top-row");
                row.AddCssClass("custom-fee-row");
                // add title text
                var cell = new TagBuilder("td");
                cell.InnerHtml += helper.DisplayNameFor(modelItem => model[i].Title);
                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                // add title editor
                cell = new TagBuilder("td");
                cell.InnerHtml += helper.EditorFor(modelItem => model[i].Title, new { htmlAttributes = new { @autocomplete = "off" } });
                // add title validation message
                {
                    var validationDiv = new TagBuilder("div");
                    validationDiv.InnerHtml += helper.ValidationMessage("[" + i + "].Title");
                    cell.InnerHtml += validationDiv.ToString(TagRenderMode.Normal);
                }
                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                // add installment text
                cell = new TagBuilder("td");
                cell.InnerHtml += helper.DisplayNameFor(modelItem => model[i].InstallmentCount);
                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                // add installment editor
                cell = new TagBuilder("td");
                cell.InnerHtml += helper.Select(modelItem => model[i].InstallmentCount, HelperUtilities.CreateNumericSelectList(1, 24, model[i].InstallmentCount));
                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                // finish the row
                rows.Append(row.ToString(TagRenderMode.Normal));
                row = new TagBuilder("tr");
                row.AddCssClass("custom-fee-bottom-row");
                row.AddCssClass("custom-fee-row");
                // add price text
                cell = new TagBuilder("td");
                cell.InnerHtml += helper.DisplayNameFor(modelItem => model[i].Price);
                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                // add price editor
                cell = new TagBuilder("td");
                cell.InnerHtml += helper.EditorFor(modelItem => model[i].Price);
                // add price validation message
                {
                    var validationDiv = new TagBuilder("div");
                    validationDiv.InnerHtml += helper.ValidationMessage("[" + i + "].Price");
                    cell.InnerHtml += validationDiv.ToString(TagRenderMode.Normal);
                }
                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                // add remove button
                cell = new TagBuilder("td");
                cell.MergeAttribute("colspan", "2");
                cell.AddCssClass("centered");
                var removeButton = new TagBuilder("input");
                removeButton.AddCssClass("link-button");
                removeButton.AddCssClass("iconed-button");
                removeButton.AddCssClass("remove-button");
                removeButton.MergeAttribute("type", "button");
                removeButton.MergeAttribute("value", RadiusR.Localization.Pages.Common.Remove);
                cell.InnerHtml += removeButton.ToString(TagRenderMode.SelfClosing);
                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                // finish the row
                rows.Append(row.ToString(TagRenderMode.Normal));
            }
            // create add row button
            {
                var row = new TagBuilder("tr");
                var cell = new TagBuilder("td");
                cell.MergeAttribute("colspan", "4");
                cell.AddCssClass("add-row-container");
                // add row button
                var addRowButton = new TagBuilder("input");
                addRowButton.MergeAttribute("type", "button");
                addRowButton.AddCssClass("link-button");
                addRowButton.AddCssClass("iconed-button");
                addRowButton.AddCssClass("add-instance-button");
                addRowButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.AddInstance);
                cell.InnerHtml += addRowButton.ToString(TagRenderMode.SelfClosing);
                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                rows.Append(row.ToString(TagRenderMode.Normal));
            }

            return new MvcHtmlString(rows.ToString());
        }
    }
}