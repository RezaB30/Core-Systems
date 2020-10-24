using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR_Manager.Helpers
{
    public static class ExpiredPoolListHelper
    {
        public static MvcHtmlString ExpiredPoolListFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : IEnumerable<ExpiredPoolViewModel>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var Model = ((IEnumerable<ExpiredPoolViewModel>)metadata.Model).ToArray();

            var rowsGrouping = new TagBuilder("tbody");
            rowsGrouping.AddCssClass("expired-pools-container");
            var rows = new StringBuilder("");
            //// add model prefix
            //{
            //    var nameContainer = new TagBuilder("input");
            //    nameContainer.MergeAttribute("type", "hidden");
            //    nameContainer.GenerateId("expired-pools-model-name");
            //    nameContainer.MergeAttribute("value", fullName);
            //    rows.Append(nameContainer.ToString(TagRenderMode.SelfClosing));
            //}
            // sample
            {
                var sample = new ExpiredPoolViewModel();
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("expired-list-sample-item");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => sample.ExpiredPoolName).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => sample.ExpiredPoolName, new { htmlAttributes = new { @maxlength = 64, @autocomplete = "off" } }).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.LabelFor(model => sample.LocalIPSubnet).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.EditorFor(model => sample.LocalIPSubnet, new { htmlAttributes = new { @maxlength = 64, @autocomplete = "off" } }).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("style", "text-align: right;");
                    TagBuilder deleteButton = new TagBuilder("input");
                    deleteButton.MergeAttribute("type", "button");
                    deleteButton.AddCssClass("remove-button iconed-button link-button");
                    deleteButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.Remove);
                    cell.InnerHtml = deleteButton.ToString(TagRenderMode.SelfClosing);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                rows.Append(row.ToString(TagRenderMode.Normal));
            }
            // list
            for (int i = 0; i < Model.Count(); i++)
            {
                // editor row
                {
                    TagBuilder row = new TagBuilder("tr");
                    row.AddCssClass("expired-list-item");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(modelItem => Model[i].ExpiredPoolName).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.EditorFor(modelItem => Model[i].ExpiredPoolName, new { htmlAttributes = new { @maxlength = 64, @autocomplete = "off" } }).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(modelItem => Model[i].LocalIPSubnet).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.EditorFor(modelItem => Model[i].LocalIPSubnet, new { htmlAttributes = new { @maxlength = 64, @autocomplete = "off" } }).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("style", "text-align: right;");
                        TagBuilder deleteButton = new TagBuilder("input");
                        deleteButton.MergeAttribute("type", "button");
                        deleteButton.AddCssClass("remove-button iconed-button link-button");
                        deleteButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.Remove);
                        cell.InnerHtml = deleteButton.ToString(TagRenderMode.SelfClosing);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    rows.Append(row.ToString(TagRenderMode.Normal));
                }
                // validation row
                {
                    TagBuilder row = new TagBuilder("tr");
                    row.AddCssClass("expired-list-item");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "2");
                        cell.InnerHtml = helper.ValidationMessageFor(modelItem => Model[i].ExpiredPoolName).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "2");
                        cell.InnerHtml = helper.ValidationMessageFor(modelItem => Model[i].LocalIPSubnet).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    rows.Append(row.ToString(TagRenderMode.Normal));
                }

            }
            // add row
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("expired-pool-add-row");
                TagBuilder cell = new TagBuilder("td");
                cell.MergeAttribute("colspan", "5");
                TagBuilder addButton = new TagBuilder("input");
                addButton.MergeAttribute("type", "button");
                addButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.AddInstance);
                addButton.AddCssClass("add-instance-button iconed-button link-button");
                cell.InnerHtml = addButton.ToString(TagRenderMode.SelfClosing);
                row.InnerHtml = cell.ToString(TagRenderMode.Normal);
                rows.Append(row.ToString(TagRenderMode.Normal));
            }

            rowsGrouping.InnerHtml = rows.ToString();

            return new MvcHtmlString(rowsGrouping.ToString(TagRenderMode.Normal));
        }
    }
}