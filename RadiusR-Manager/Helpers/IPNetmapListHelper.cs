using RadiusR_Manager.Models.RadiusViewModels;
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
    public static class IPNetmapListHelper
    {
        public static MvcHtmlString IPNetmapList<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TModel : IEnumerable<NASNetmapViewModel>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = ((IEnumerable<NASNetmapViewModel>)metadata.Model).ToArray();

            var rowsGrouping = new TagBuilder("tbody");
            rowsGrouping.AddCssClass("ip-map-container");
            var rows = new StringBuilder("");
            // add model prefix
            {
                var nameContainer = new TagBuilder("input");
                nameContainer.MergeAttribute("type", "hidden");
                nameContainer.GenerateId("ip-map-model-name");
                nameContainer.MergeAttribute("value", fullName);
                rows.Append(nameContainer.ToString(TagRenderMode.SelfClosing));
            }
            {
                // create the sample row
                var sampleModel = new NASNetmapViewModel();
                {
                    {
                        var row = new TagBuilder("tr");
                        row.AddCssClass("ip-map-list-row");
                        row.AddCssClass("ip-map-list-sample-row");
                        row.AddCssClass("ip-map-list-top-row");
                        {
                            // from part
                            var labelCell = new TagBuilder("td");
                            labelCell.InnerHtml += helper.DisplayNameFor(modelItem => sampleModel.LocalIPSubnet);
                            var editorCell = new TagBuilder("td");
                            editorCell.InnerHtml += helper.EditorFor(modelItem => sampleModel.LocalIPSubnet, new { htmlAttributes = new { @autocomplete = "off", @maxlength = 18 } });
                            editorCell.InnerHtml += helper.CheckButton(modelItem => sampleModel.PreserveLastByte, helper.DisplayNameFor(modelItem => sampleModel.PreserveLastByte).ToString());
                            row.InnerHtml += labelCell.ToString(TagRenderMode.Normal);
                            row.InnerHtml += editorCell.ToString(TagRenderMode.Normal);
                        }
                        {
                            // to part
                            var labelCell = new TagBuilder("td");
                            labelCell.InnerHtml += helper.DisplayNameFor(modelItem => sampleModel.RealIPSubnet);
                            var editorCell = new TagBuilder("td");
                            editorCell.InnerHtml += helper.EditorFor(modelItem => sampleModel.RealIPSubnet, new { htmlAttributes = new { @autocomplete = "off", @maxlength = 18 } });
                            row.InnerHtml += labelCell.ToString(TagRenderMode.Normal);
                            row.InnerHtml += editorCell.ToString(TagRenderMode.Normal);
                        }
                        {
                            // port count part
                            var labelCell = new TagBuilder("td");
                            labelCell.InnerHtml += helper.DisplayNameFor(modelItem => sampleModel.PortCount);
                            var editorCell = new TagBuilder("td");
                            editorCell.InnerHtml += helper.EditorFor(modelItem => sampleModel.PortCount, new { htmlAttributes = new { @autocomplete = "off", @maxlength = 5, @placeholder= "50-5000" } });
                            row.InnerHtml += labelCell.ToString(TagRenderMode.Normal);
                            row.InnerHtml += editorCell.ToString(TagRenderMode.Normal);
                        }
                        rows.Append(row.ToString(TagRenderMode.Normal));
                    }
                    {
                        var row = new TagBuilder("tr");
                        row.AddCssClass("ip-map-list-row");
                        row.AddCssClass("ip-map-list-sample-row");
                        {
                            // from validation
                            var validationCell = new TagBuilder("td");
                            validationCell.MergeAttribute("colspan", "2");
                            validationCell.AddCssClass("ip-map-list-validation");
                            validationCell.InnerHtml += helper.ValidationMessage("");
                            row.InnerHtml += validationCell.ToString(TagRenderMode.Normal);
                        }
                        {
                            // to validation
                            var validationCell = new TagBuilder("td");
                            validationCell.MergeAttribute("colspan", "2");
                            validationCell.AddCssClass("ip-map-list-validation");
                            validationCell.InnerHtml += helper.ValidationMessage("");
                            row.InnerHtml += validationCell.ToString(TagRenderMode.Normal);
                        }
                        {
                            // port count validation
                            var validationCell = new TagBuilder("td");
                            validationCell.MergeAttribute("colspan", "2");
                            validationCell.AddCssClass("ip-map-list-validation");
                            validationCell.InnerHtml += helper.ValidationMessage("");
                            row.InnerHtml += validationCell.ToString(TagRenderMode.Normal);
                        }
                        rows.Append(row.ToString(TagRenderMode.Normal));
                    }
                    {
                        var row = new TagBuilder("tr");
                        row.AddCssClass("ip-map-list-row");
                        row.AddCssClass("ip-map-list-sample-row");
                        row.AddCssClass("ip-map-list-bottom-row");
                        {
                            // delete button part
                            var setValidationCell = new TagBuilder("td");
                            setValidationCell.MergeAttribute("colspan", "5");
                            var removeCell = new TagBuilder("td");
                            var deleteButton = new TagBuilder("input");
                            deleteButton.MergeAttribute("type", "button");
                            deleteButton.MergeAttribute("value", RadiusR.Localization.Pages.Common.Remove);
                            deleteButton.AddCssClass("link-button");
                            deleteButton.AddCssClass("iconed-button");
                            deleteButton.AddCssClass("remove-button");
                            removeCell.InnerHtml += deleteButton.ToString(TagRenderMode.SelfClosing);
                            row.InnerHtml += setValidationCell.ToString(TagRenderMode.Normal);
                            row.InnerHtml += removeCell.ToString(TagRenderMode.Normal);
                        }
                        rows.Append(row.ToString(TagRenderMode.Normal));
                    }
                }
            }
            // create existing data rows
            for (int i = 0; i < model.Count(); i++)
            {
                {
                    var row = new TagBuilder("tr");
                    row.AddCssClass("ip-map-list-row");
                    row.AddCssClass("ip-map-list-top-row");
                    {
                        // from part
                        var labelCell = new TagBuilder("td");
                        labelCell.InnerHtml += helper.DisplayNameFor(modelItem => model[i].LocalIPSubnet);
                        var editorCell = new TagBuilder("td");
                        editorCell.InnerHtml += helper.EditorFor(modelItem => model[i].LocalIPSubnet, new { htmlAttributes = new { @autocomplete = "off", @maxlength = 18 } });
                        editorCell.InnerHtml += helper.CheckButton(modelItem => model[i].PreserveLastByte, helper.DisplayNameFor(modelItem => model[i].PreserveLastByte).ToString());
                        row.InnerHtml += labelCell.ToString(TagRenderMode.Normal);
                        row.InnerHtml += editorCell.ToString(TagRenderMode.Normal);
                    }
                    {
                        // to part
                        var labelCell = new TagBuilder("td");
                        labelCell.InnerHtml += helper.DisplayNameFor(modelItem => model[i].RealIPSubnet);
                        var editorCell = new TagBuilder("td");
                        editorCell.InnerHtml += helper.EditorFor(modelItem => model[i].RealIPSubnet, new { htmlAttributes = new { @autocomplete = "off", @maxlength = 18 } });
                        row.InnerHtml += labelCell.ToString(TagRenderMode.Normal);
                        row.InnerHtml += editorCell.ToString(TagRenderMode.Normal);
                    }
                    {
                        // port count part
                        var labelCell = new TagBuilder("td");
                        labelCell.InnerHtml += helper.DisplayNameFor(modelItem => model[i].PortCount);
                        var editorCell = new TagBuilder("td");
                        editorCell.InnerHtml += helper.EditorFor(modelItem => model[i].PortCount, new { htmlAttributes = new { @autocomplete = "off", @maxlength = 5, @placeholder = "50-5000" } });
                        row.InnerHtml += labelCell.ToString(TagRenderMode.Normal);
                        row.InnerHtml += editorCell.ToString(TagRenderMode.Normal);
                    }
                    rows.Append(row.ToString(TagRenderMode.Normal));
                }
                {
                    var row = new TagBuilder("tr");
                    row.AddCssClass("ip-map-list-row");
                    {
                        // from validation
                        var validationCell = new TagBuilder("td");
                        validationCell.MergeAttribute("colspan", "2");
                        validationCell.AddCssClass("ip-map-list-validation");
                        validationCell.InnerHtml += helper.ValidationMessage("[" + i + "].LocalIPSubnet");
                        row.InnerHtml += validationCell.ToString(TagRenderMode.Normal);
                    }
                    {
                        // to validation
                        var validationCell = new TagBuilder("td");
                        validationCell.MergeAttribute("colspan", "2");
                        validationCell.AddCssClass("ip-map-list-validation");
                        validationCell.InnerHtml += helper.ValidationMessage("[" + i + "].RealIPSubnet");
                        row.InnerHtml += validationCell.ToString(TagRenderMode.Normal);
                    }
                    {
                        // port count validation
                        var validationCell = new TagBuilder("td");
                        validationCell.MergeAttribute("colspan", "2");
                        validationCell.AddCssClass("ip-map-list-validation");
                        validationCell.InnerHtml += helper.ValidationMessage("[" + i + "].PortCount");
                        row.InnerHtml += validationCell.ToString(TagRenderMode.Normal);
                    }
                    rows.Append(row.ToString(TagRenderMode.Normal));
                }
                {
                    var row = new TagBuilder("tr");
                    row.AddCssClass("ip-map-list-row");
                    row.AddCssClass("ip-map-list-bottom-row");
                    {
                        // delete button & set validation part
                        var setValidationCell = new TagBuilder("td");
                        setValidationCell.MergeAttribute("colspan", "5");
                        setValidationCell.InnerHtml += helper.ValidationMessage("[" + i + "].Set");
                        var removeCell = new TagBuilder("td");
                        var deleteButton = new TagBuilder("input");
                        deleteButton.MergeAttribute("type", "button");
                        deleteButton.MergeAttribute("value", RadiusR.Localization.Pages.Common.Remove);
                        deleteButton.AddCssClass("link-button");
                        deleteButton.AddCssClass("iconed-button");
                        deleteButton.AddCssClass("remove-button");
                        removeCell.InnerHtml += deleteButton.ToString(TagRenderMode.SelfClosing);
                        row.InnerHtml += setValidationCell.ToString(TagRenderMode.Normal);
                        row.InnerHtml += removeCell.ToString(TagRenderMode.Normal);
                    }
                    rows.Append(row.ToString(TagRenderMode.Normal));
                }
            }
            // add button row
            {
                var row = new TagBuilder("tr");
                row.AddCssClass("ip-map-list-row");
                var addButtonCell = new TagBuilder("td");
                addButtonCell.MergeAttribute("colspan", "6");
                var addButton = new TagBuilder("input");
                addButton.MergeAttribute("type", "button");
                addButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.AddInstance);
                addButton.AddCssClass("link-button");
                addButton.AddCssClass("iconed-button");
                addButton.AddCssClass("add-instance-button");
                addButtonCell.InnerHtml += addButton.ToString(TagRenderMode.SelfClosing);
                row.InnerHtml += addButtonCell.ToString(TagRenderMode.Normal);
                rows.Append(row.ToString(TagRenderMode.Normal));
            }

            rowsGrouping.InnerHtml += rows.ToString();
            return new MvcHtmlString(rowsGrouping.ToString(TagRenderMode.Normal));
        }
    }
}