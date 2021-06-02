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
    public static class VerticalDSLIPMapListHelper
    {
        public static MvcHtmlString VerticalDSLIPMapEditorFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TModel : VerticalDSLIPMapViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = (VerticalDSLIPMapViewModel)metadata.Model;

            // local IP rows
            var localIPRowsGrouping = new TagBuilder("tbody");
            localIPRowsGrouping.AddCssClass("ip-map-container");
            var localIPRows = new StringBuilder("");
            // DSL IP rows
            var dslIPRowsGrouping = new TagBuilder("tbody");
            dslIPRowsGrouping.AddCssClass("ip-map-container");
            var dslIPRows = new StringBuilder("");
            // sample models
            var sampleLocalIPModel = new VerticalDSLIPMapViewModel.IPSubnet();
            var sampleDSLIPModel = new VerticalDSLIPMapViewModel.IP();
            // local IP parts
            {
                // title row
                {
                    var row = new TagBuilder("tr");
                    var cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "3");
                    var title = new TagBuilder("div");
                    title.AddCssClass("rate-limit-title");
                    title.InnerHtml += helper.DisplayNameFor(modelItem => model.LocalIPSubnets);
                    cell.InnerHtml += title.ToString(TagRenderMode.Normal);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    row.InnerHtml += helper.Hidden("prefixName", helper.NameFor(modelItem => model.LocalIPSubnets));
                    localIPRows.Append(row.ToString(TagRenderMode.Normal));
                }
                // sample row
                {
                    var row = new TagBuilder("tr");
                    row.AddCssClass("ip-map-list-row");
                    row.AddCssClass("ip-map-list-sample-row");
                    {
                        var labelCell = new TagBuilder("td");
                        labelCell.InnerHtml += helper.DisplayNameFor(modelItem => sampleLocalIPModel.Value);
                        var editorCell = new TagBuilder("td");
                        editorCell.InnerHtml += helper.EditorFor(modelItem => sampleLocalIPModel.Value, new { htmlAttributes = new { @autocomplete = "off", @maxlength = 18 } });
                        var commandCell = new TagBuilder("td");
                        var deleteButton = new TagBuilder("input");
                        deleteButton.MergeAttribute("type", "button");
                        deleteButton.MergeAttribute("value", RadiusR.Localization.Pages.Common.Remove);
                        deleteButton.AddCssClass("link-button");
                        deleteButton.AddCssClass("iconed-button");
                        deleteButton.AddCssClass("remove-button");
                        commandCell.InnerHtml += deleteButton.ToString(TagRenderMode.SelfClosing);

                        row.InnerHtml += labelCell.ToString(TagRenderMode.Normal);
                        row.InnerHtml += editorCell.ToString(TagRenderMode.Normal);
                        row.InnerHtml += commandCell.ToString(TagRenderMode.Normal);
                    }

                    localIPRows.Append(row.ToString(TagRenderMode.Normal));
                }
                // existing values
                if (model?.LocalIPSubnets != null)
                {
                    for (int i = 0; i < model.LocalIPSubnets.Count(); i++)
                    {
                        var row = new TagBuilder("tr");
                        row.AddCssClass("ip-map-list-row");
                        {
                            var labelCell = new TagBuilder("td");
                            labelCell.InnerHtml += helper.DisplayNameFor(modelItem => model.LocalIPSubnets[i].Value);
                            var editorCell = new TagBuilder("td");
                            editorCell.InnerHtml += helper.EditorFor(modelItem => model.LocalIPSubnets[i].Value, new { htmlAttributes = new { @autocomplete = "off", @maxlength = 18 } });
                            editorCell.InnerHtml += new MvcHtmlString("&nbsp;");
                            editorCell.InnerHtml += helper.ValidationMessageFor(modelItem => model.LocalIPSubnets[i].Value);
                            var commandCell = new TagBuilder("td");
                            var deleteButton = new TagBuilder("input");
                            deleteButton.MergeAttribute("type", "button");
                            deleteButton.MergeAttribute("value", RadiusR.Localization.Pages.Common.Remove);
                            deleteButton.AddCssClass("link-button");
                            deleteButton.AddCssClass("iconed-button");
                            deleteButton.AddCssClass("remove-button");
                            commandCell.InnerHtml += deleteButton.ToString(TagRenderMode.SelfClosing);

                            row.InnerHtml += labelCell.ToString(TagRenderMode.Normal);
                            row.InnerHtml += editorCell.ToString(TagRenderMode.Normal);
                            row.InnerHtml += commandCell.ToString(TagRenderMode.Normal);
                        }

                        localIPRows.Append(row.ToString(TagRenderMode.Normal));
                    }
                }
                // add button row
                {
                    var row = new TagBuilder("tr");
                    row.AddCssClass("ip-map-list-row");
                    var addButtonCell = new TagBuilder("td");
                    addButtonCell.MergeAttribute("colspan", "3");
                    var addButton = new TagBuilder("input");
                    addButton.MergeAttribute("type", "button");
                    addButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.AddInstance);
                    addButton.AddCssClass("link-button");
                    addButton.AddCssClass("iconed-button");
                    addButton.AddCssClass("add-instance-button");
                    addButtonCell.InnerHtml += addButton.ToString(TagRenderMode.SelfClosing);
                    row.InnerHtml += addButtonCell.ToString(TagRenderMode.Normal);
                    localIPRows.Append(row.ToString(TagRenderMode.Normal));
                }
                localIPRowsGrouping.InnerHtml += localIPRows.ToString();
            }
            // DSL IP rows
            {
                // title row
                {
                    var row = new TagBuilder("tr");
                    var cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "3");
                    var title = new TagBuilder("div");
                    title.AddCssClass("rate-limit-title");
                    title.InnerHtml += helper.DisplayNameFor(modelItem => model.DSLLines);
                    cell.InnerHtml += title.ToString(TagRenderMode.Normal);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    row.InnerHtml += helper.Hidden("prefixName", helper.NameFor(modelItem => model.DSLLines));
                    dslIPRows.Append(row.ToString(TagRenderMode.Normal));
                }
                // sample row
                {
                    var row = new TagBuilder("tr");
                    row.AddCssClass("ip-map-list-row");
                    row.AddCssClass("ip-map-list-sample-row");
                    {
                        var labelCell = new TagBuilder("td");
                        labelCell.InnerHtml += helper.DisplayNameFor(modelItem => sampleDSLIPModel.Value);
                        var editorCell = new TagBuilder("td");
                        editorCell.InnerHtml += helper.EditorFor(modelItem => sampleDSLIPModel.Value, new { htmlAttributes = new { @autocomplete = "off", @maxlength = 15 } });
                        var commandCell = new TagBuilder("td");
                        var deleteButton = new TagBuilder("input");
                        deleteButton.MergeAttribute("type", "button");
                        deleteButton.MergeAttribute("value", RadiusR.Localization.Pages.Common.Remove);
                        deleteButton.AddCssClass("link-button");
                        deleteButton.AddCssClass("iconed-button");
                        deleteButton.AddCssClass("remove-button");
                        commandCell.InnerHtml += deleteButton.ToString(TagRenderMode.SelfClosing);

                        row.InnerHtml += labelCell.ToString(TagRenderMode.Normal);
                        row.InnerHtml += editorCell.ToString(TagRenderMode.Normal);
                        row.InnerHtml += commandCell.ToString(TagRenderMode.Normal);
                    }

                    dslIPRows.Append(row.ToString(TagRenderMode.Normal));
                }
                // existing values
                if (model?.DSLLines != null)
                {
                    for (int i = 0; i < model.DSLLines.Count(); i++)
                    {
                        var row = new TagBuilder("tr");
                        row.AddCssClass("ip-map-list-row");
                        {
                            var labelCell = new TagBuilder("td");
                            labelCell.InnerHtml += helper.DisplayNameFor(modelItem => model.DSLLines[i].Value);
                            var editorCell = new TagBuilder("td");
                            editorCell.InnerHtml += helper.EditorFor(modelItem => model.DSLLines[i].Value, new { htmlAttributes = new { @autocomplete = "off", @maxlength = 18 } });
                            editorCell.InnerHtml += new MvcHtmlString("&nbsp;");
                            editorCell.InnerHtml += helper.ValidationMessageFor(modelItem => model.DSLLines[i].Value);
                            var commandCell = new TagBuilder("td");
                            var deleteButton = new TagBuilder("input");
                            deleteButton.MergeAttribute("type", "button");
                            deleteButton.MergeAttribute("value", RadiusR.Localization.Pages.Common.Remove);
                            deleteButton.AddCssClass("link-button");
                            deleteButton.AddCssClass("iconed-button");
                            deleteButton.AddCssClass("remove-button");
                            commandCell.InnerHtml += deleteButton.ToString(TagRenderMode.SelfClosing);

                            row.InnerHtml += labelCell.ToString(TagRenderMode.Normal);
                            row.InnerHtml += editorCell.ToString(TagRenderMode.Normal);
                            row.InnerHtml += commandCell.ToString(TagRenderMode.Normal);
                        }

                        dslIPRows.Append(row.ToString(TagRenderMode.Normal));
                    }
                }
                // add button row
                {
                    var row = new TagBuilder("tr");
                    row.AddCssClass("ip-map-list-row");
                    var addButtonCell = new TagBuilder("td");
                    addButtonCell.MergeAttribute("colspan", "3");
                    var addButton = new TagBuilder("input");
                    addButton.MergeAttribute("type", "button");
                    addButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.AddInstance);
                    addButton.AddCssClass("link-button");
                    addButton.AddCssClass("iconed-button");
                    addButton.AddCssClass("add-instance-button");
                    addButtonCell.InnerHtml += addButton.ToString(TagRenderMode.SelfClosing);
                    row.InnerHtml += addButtonCell.ToString(TagRenderMode.Normal);
                    dslIPRows.Append(row.ToString(TagRenderMode.Normal));
                }
                dslIPRowsGrouping.InnerHtml += dslIPRows.ToString();
            }

            return new MvcHtmlString(localIPRowsGrouping.ToString(TagRenderMode.Normal) + dslIPRowsGrouping.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString VerticalDSLIPMapDisplayFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TModel : VerticalDSLIPMapViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = (VerticalDSLIPMapViewModel)metadata.Model;

            var wrapper = new TagBuilder("div");
            wrapper.AddCssClass("vertical-dsl-ip-map-wrapper");
            {
                // local subnets
                {
                    var localIPDiv = new TagBuilder("div");
                    localIPDiv.AddCssClass("vertical-dsl-ip-map-part");
                    if (model?.LocalIPSubnets?.Any() == true)
                    {
                        foreach (var item in model.LocalIPSubnets)
                        {
                            var row = new TagBuilder("div");
                            row.AddCssClass("row-item");
                            row.InnerHtml += helper.DisplayFor(modelItem => item.Value);
                            localIPDiv.InnerHtml += row.ToString(TagRenderMode.Normal);
                        }
                    }
                    wrapper.InnerHtml += localIPDiv.ToString(TagRenderMode.Normal);
                }
                wrapper.InnerHtml += new HtmlString("&gt;");
                // dsl IPs
                {
                    var dslIPDiv = new TagBuilder("div");
                    dslIPDiv.AddCssClass("vertical-dsl-ip-map-part");
                    if (model?.DSLLines?.Any() == true)
                    {
                        foreach (var item in model.DSLLines)
                        {
                            var row = new TagBuilder("div");
                            row.AddCssClass("row-item");
                            row.InnerHtml += helper.DisplayFor(modelItem => item.Value);
                            dslIPDiv.InnerHtml += row.ToString(TagRenderMode.Normal);
                        }
                    }
                    wrapper.InnerHtml += dslIPDiv.ToString(TagRenderMode.Normal);
                }
            }

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}