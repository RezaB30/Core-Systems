using RadiusR_Manager.Models.ViewModels;
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
    public static class ServiceDomainHelper
    {
        public static MvcHtmlString ServiceDomainEditorFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, IEnumerable<ServiceDomainViewModel> AllDomains) where TResult : IEnumerable<ServiceDomainViewModel>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var value = metadata.Model as IEnumerable<ServiceDomainViewModel>;

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("service-domain-editor-wrapper");

            // constant values
            TagBuilder constantValuesWrapper = new TagBuilder("div");
            TagBuilder ol = new TagBuilder("ol");

            var constantValues = value.Where(d => !d.CanBeChanged).ToArray();
            foreach (var item in constantValues)
            {
                TagBuilder li = new TagBuilder("li");
                li.InnerHtml = helper.DisplayFor(model => item.DomainName).ToHtmlString();
                ol.InnerHtml += li.ToString(TagRenderMode.Normal);
            }

            constantValuesWrapper.InnerHtml = ol.ToString(TagRenderMode.Normal);
            wrapper.InnerHtml += constantValuesWrapper.ToString(TagRenderMode.Normal);

            // changing values

            var changeableValues = new MultiSelectList(AllDomains.Where(d => !constantValues.Select(cv => cv.DomainID).Contains(d.DomainID)), "DomainID", "DomainName", value.Where(d => d.CanBeChanged).Select(d => d.DomainID));
            wrapper.InnerHtml += helper.MultiSelect("DomainIDs", changeableValues, RadiusR.Localization.Helpers.Common.Select);

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString ServiceDomainDisplayFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : IEnumerable<ServiceDomainViewModel>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var value = metadata.Model as IEnumerable<ServiceDomainViewModel>;

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("service-domain-display-wrapper");
            foreach (var item in value)
            {
                TagBuilder div = new TagBuilder("div");
                div.InnerHtml = helper.DisplayFor(model => item.DomainName).ToHtmlString();
                wrapper.InnerHtml += div.ToString(TagRenderMode.Normal);
            }

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}