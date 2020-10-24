using RadiusR_Manager.Models.ViewModels.Customer;
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
    public static class PhoneNoListHelper
    {
        public static MvcHtmlString PhoneNoListEditorFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : IEnumerable<CustomerGeneralInfoViewModel.PhoneNo>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = ((IEnumerable<CustomerGeneralInfoViewModel.PhoneNo>)metadata.Model ?? Enumerable.Empty<CustomerGeneralInfoViewModel.PhoneNo>()).ToArray();

            // wrapper
            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("phone-no-list-wrapper");
            // sample row
            {
                var sample = new CustomerGeneralInfoViewModel.PhoneNo();
                TagBuilder sampleContainer = new TagBuilder("div");
                sampleContainer.MergeAttribute("style", "display: none;");
                sampleContainer.AddCssClass("sample-row");


                TagBuilder li = new TagBuilder("li");
                li.AddCssClass("multiselect-input-row");
                li.InnerHtml = helper.EditorFor(modelItem => sample.Number, new { htmlAttributes = new { @maxlength = 10, @autocomplete = "off" } }).ToHtmlString();

                TagBuilder removeButton = new TagBuilder("input");
                removeButton.MergeAttribute("type", "button");
                removeButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.Remove);
                removeButton.AddCssClass("link-button iconed-button remove-instance-button");
                li.InnerHtml += " " + removeButton.ToString(TagRenderMode.SelfClosing);

                sampleContainer.InnerHtml = li.ToString(TagRenderMode.Normal);
                wrapper.InnerHtml += sampleContainer.ToString(TagRenderMode.Normal);
            }
            // value list
            {
                TagBuilder ol = new TagBuilder("ol");
                ol.AddCssClass("multiselect-orderedlist");

                for (int i = 0; i < model.Count(); i++)
                {
                    TagBuilder li = new TagBuilder("li");
                    li.AddCssClass("multiselect-input-row");

                    {
                        TagBuilder itemRow = new TagBuilder("div");
                        itemRow.InnerHtml = helper.EditorFor(modelItem => model[i].Number, new { htmlAttributes = new { @maxlength = 10, @autocomplete = "off" } }).ToHtmlString();

                        TagBuilder removeButton = new TagBuilder("input");
                        removeButton.MergeAttribute("type", "button");
                        removeButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.Remove);
                        removeButton.AddCssClass("link-button iconed-button remove-instance-button");
                        itemRow.InnerHtml += " " + removeButton.ToString(TagRenderMode.SelfClosing);

                        li.InnerHtml += itemRow.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder validationRow = new TagBuilder("div");
                        validationRow.InnerHtml = helper.ValidationMessageFor(modelItem => model[i].Number).ToHtmlString();
                        li.InnerHtml += validationRow.ToString(TagRenderMode.Normal);
                    }

                    ol.InnerHtml += li.ToString(TagRenderMode.Normal);
                }
                wrapper.InnerHtml += ol.ToString(TagRenderMode.Normal);
            }
            // add row
            {
                TagBuilder addRow = new TagBuilder("div");
                addRow.AddCssClass("add-row");

                TagBuilder addButton = new TagBuilder("input");
                addButton.MergeAttribute("type", "button");
                addButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.AddInstance);
                addButton.AddCssClass("linked-button iconed-button add-instance-button");

                addRow.InnerHtml += addButton.ToString(TagRenderMode.SelfClosing);
                wrapper.InnerHtml += addRow.ToString(TagRenderMode.Normal);
            }

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString PhoneNoListDisplayFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : IEnumerable<CustomerGeneralInfoViewModel.PhoneNo>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = ((IEnumerable<CustomerGeneralInfoViewModel.PhoneNo>)metadata.Model ?? Enumerable.Empty<CustomerGeneralInfoViewModel.PhoneNo>()).ToArray();

            TagBuilder wrapper = new TagBuilder("div");

            foreach (var item in model)
            {
                TagBuilder row = new TagBuilder("div");
                row.InnerHtml = helper.DisplayFor(modelItem => item.Number).ToHtmlString();
                wrapper.InnerHtml += row.ToString(TagRenderMode.Normal);
            }

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}
