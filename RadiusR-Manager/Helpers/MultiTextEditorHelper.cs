using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR_Manager.Helpers
{
    public static class MultiTextEditorHelper
    {
        public static MvcHtmlString MultiTextEditorFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, object htmlAttributes = null) where TResult : IEnumerable<string>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var Model = (IEnumerable<string>)metadata.Model;

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("multi-text-editor-wrapper");
            // sample
            {
                var sampleContainer = new TagBuilder("div");
                sampleContainer.AddCssClass("multi-text-editor-sample");
                sampleContainer.MergeAttribute("style", "display: none;");
                var sampleInput = new TagBuilder("input");
                sampleInput.MergeAttribute("name", fullName);
                if (htmlAttributes != null)
                    foreach (var attr in htmlAttributes.GetType().GetProperties())
                    {
                        sampleInput.MergeAttribute(attr.Name, attr.GetValue(htmlAttributes).ToString());
                    }
                sampleContainer.InnerHtml = sampleInput.ToString(TagRenderMode.SelfClosing);
                // remove button
                var removeButton = new TagBuilder("input");
                removeButton.MergeAttribute("value", RezaB.Web.Helpers.Localization.Common.Remove);
                removeButton.MergeAttribute("type", "button");
                removeButton.AddCssClass("link-button iconed-button remove-instance-button multi-text-editor-list-item-remove");
                sampleContainer.InnerHtml += "&nbsp;";
                sampleContainer.InnerHtml += removeButton.ToString(TagRenderMode.SelfClosing);

                wrapper.InnerHtml += sampleContainer.ToString(TagRenderMode.Normal);
            }
            // item list
            {
                var itemListContainer = new TagBuilder("div");
                itemListContainer.AddCssClass("multi-text-editor-list");
                {
                    var orderedList = new TagBuilder("ol");
                    orderedList.AddCssClass("multiselect-orderedlist");
                    // add current items
                    foreach (var item in Model)
                    {
                        var listItem = new TagBuilder("li");
                        listItem.AddCssClass("multiselect-input-row");
                        var itemInput = new TagBuilder("input");
                        itemInput.MergeAttribute("name", fullName);
                        itemInput.MergeAttribute("value", item);
                        if (htmlAttributes != null)
                            foreach (var attr in htmlAttributes.GetType().GetProperties())
                            {
                                itemInput.MergeAttribute(attr.Name, attr.GetValue(htmlAttributes).ToString());
                            }
                        listItem.InnerHtml += itemInput.ToString(TagRenderMode.SelfClosing);
                        listItem.InnerHtml += "&nbsp;";
                        // remove button
                        var removeButton = new TagBuilder("input");
                        removeButton.MergeAttribute("value", RezaB.Web.Helpers.Localization.Common.Remove);
                        removeButton.MergeAttribute("type", "button");
                        removeButton.AddCssClass("link-button iconed-button remove-instance-button multi-text-editor-list-item-remove");
                        listItem.InnerHtml += removeButton.ToString(TagRenderMode.SelfClosing);

                        orderedList.InnerHtml += listItem.ToString(TagRenderMode.Normal);
                    }

                    itemListContainer.InnerHtml += orderedList.ToString(TagRenderMode.Normal);
                }

                wrapper.InnerHtml += itemListContainer.ToString(TagRenderMode.Normal);
            }
            // add
            {
                var addContainer = new TagBuilder("div");
                addContainer.AddCssClass("multi-text-editor-add-container");
                var addButton = new TagBuilder("input");
                addButton.MergeAttribute("value", RezaB.Web.Helpers.Localization.Common.AddInstance);
                addButton.MergeAttribute("type", "button");
                addButton.AddCssClass("link-button iconed-button add-instance-button multi-text-editor-add");
                addContainer.InnerHtml += addButton.ToString(TagRenderMode.SelfClosing);

                wrapper.InnerHtml += addContainer.ToString(TagRenderMode.Normal);
            }

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}