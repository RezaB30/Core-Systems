using RadiusR_Manager.Models.RadiusViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR_Manager.Helpers
{
    public static class FeeTypeVariantListHelper
    {

        public static MvcHtmlString FeeTypeVariantList<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, string modelName) where TModel : IEnumerable<FeeTypeVariantViewModel>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = metadata.Model as IEnumerable<FeeTypeVariantViewModel>;

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("fee-type-list-wrapper");
            
            // model name container
            TagBuilder modelNameContainer = new TagBuilder("input");
            modelNameContainer.MergeAttribute("type", "hidden");
            modelNameContainer.MergeAttribute("id", "name_container");
            modelNameContainer.MergeAttribute("value", modelName);
            wrapper.InnerHtml += modelNameContainer.ToString(TagRenderMode.SelfClosing);

            // sample layer
            {
                TagBuilder sample = new TagBuilder("table");
                sample.MergeAttribute("style", "display: none;");
                sample.AddCssClass("fee-type-list-sample");
                var sampleModelItem = new FeeTypeVariantViewModel();
                TagBuilder sampleRow = new TagBuilder("tr");
                sampleRow.MergeAttribute("style", "display: none;");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.LabelFor(modelItem => sampleModelItem.Title);
                    sampleRow.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.EditorFor(modelItem => sampleModelItem.Title);
                    sampleRow.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.LabelFor(modelItem => sampleModelItem.Price);
                    sampleRow.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.EditorFor(modelItem => sampleModelItem.Price);
                    sampleRow.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    TagBuilder removeButton = new TagBuilder("input");
                    removeButton.MergeAttribute("type", "button");
                    removeButton.AddCssClass("link-button iconed-button remove-button");
                    removeButton.MergeAttribute("value", RadiusR.Localization.Pages.Common.Remove);
                    cell.InnerHtml += removeButton.ToString(TagRenderMode.SelfClosing);
                    sampleRow.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                sample.InnerHtml += sampleRow.ToString(TagRenderMode.Normal);
                wrapper.InnerHtml += sample.ToString(TagRenderMode.Normal);
            }

            TagBuilder table = new TagBuilder("table");
            table.AddCssClass("input-table");

            for (int i = 0; i < model.Count(); i++)
            {
                var variants = model.ToArray();
                TagBuilder row = new TagBuilder("tr");
                row.InnerHtml += helper.HiddenFor(modelItem => variants[i].ID);
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.LabelFor(modelItem => variants[i].Title);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.EditorFor(modelItem => variants[i].Title);
                    TagBuilder validation = new TagBuilder("div");
                    validation.InnerHtml += helper.ValidationMessageFor(modelItem => variants[i].Title);
                    cell.InnerHtml += validation.ToString(TagRenderMode.Normal);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.LabelFor(modelItem => variants[i].Price);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.EditorFor(modelItem => variants[i].Price);
                    TagBuilder validation = new TagBuilder("div");
                    validation.InnerHtml += helper.ValidationMessageFor(modelItem => variants[i].Price);
                    cell.InnerHtml += validation.ToString(TagRenderMode.Normal);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    TagBuilder removeButton = new TagBuilder("input");
                    removeButton.MergeAttribute("type", "button");
                    removeButton.AddCssClass("link-button iconed-button remove-button");
                    removeButton.MergeAttribute("value", RadiusR.Localization.Pages.Common.Remove);
                    cell.InnerHtml += removeButton.ToString(TagRenderMode.SelfClosing);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }

            {
                TagBuilder row = new TagBuilder("tr");
                TagBuilder cell = new TagBuilder("td");
                cell.MergeAttribute("colspan", "4");

                var addInstanceButton = new TagBuilder("input");
                addInstanceButton.MergeAttribute("type", "button");
                addInstanceButton.AddCssClass("link-button iconed-button add-instance-button");
                addInstanceButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.AddInstance);

                cell.InnerHtml += addInstanceButton.ToString(TagRenderMode.SelfClosing);
                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }

            wrapper.InnerHtml += table.ToString(TagRenderMode.Normal);
            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}