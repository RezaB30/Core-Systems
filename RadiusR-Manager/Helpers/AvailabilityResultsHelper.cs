using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR_Manager.Helpers
{
    public static class AvailabilityResultsHelper
    {
        public static MvcHtmlString AvailabilityResultsFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TModel : AvailabilityResultsViewModel.AvailabilityResult
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var value = (AvailabilityResultsViewModel.AvailabilityResult)metadata.Model;

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("availability-results-wrapper");
            switch ((RezaB.TurkTelekom.WebServices.Availability.AvailabilityServiceClient.AvailabilityResult)value.Result)
            {

                case RezaB.TurkTelekom.WebServices.Availability.AvailabilityServiceClient.AvailabilityResult.Available:
                    wrapper.AddCssClass("available");
                    break;
                case RezaB.TurkTelekom.WebServices.Availability.AvailabilityServiceClient.AvailabilityResult.AvailableButLongDistance:
                    wrapper.AddCssClass("available-warning");
                    break;
                default:
                    wrapper.AddCssClass("unavailable");
                    break;
            }

            TagBuilder title = new TagBuilder("div");
            title.AddCssClass("availability-results-title");
            title.InnerHtml = helper.DisplayFor(model => model.XDSLType).ToHtmlString();
            wrapper.InnerHtml += title.ToString(TagRenderMode.Normal);

            TagBuilder speed = new TagBuilder("div");
            speed.AddCssClass("availability-results-speed");
            TagBuilder speedTitle = new TagBuilder("div");
            speedTitle.AddCssClass("speed-title");
            speedTitle.InnerHtml = helper.DisplayNameFor(model => model.DSLMaxSpeed).ToHtmlString();
            speed.InnerHtml += speedTitle.ToString(TagRenderMode.Normal);
            TagBuilder speedValue = new TagBuilder("div");
            speedValue.AddCssClass("speed-value");
            speedValue.InnerHtml = helper.DisplayFor(model => model.DSLMaxSpeed).ToHtmlString();
            speed.InnerHtml += speedValue.ToString(TagRenderMode.Normal);
            wrapper.InnerHtml += speed.ToString(TagRenderMode.Normal);
            TagBuilder resultsDiv = new TagBuilder("div");
            resultsDiv.AddCssClass("results");
            {
                TagBuilder div = new TagBuilder("div");
                div.AddCssClass("title");
                div.InnerHtml = helper.DisplayNameFor(model => model.Result).ToHtmlString();
                resultsDiv.InnerHtml += div.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder div = new TagBuilder("div");
                div.AddCssClass("value");
                div.InnerHtml = helper.DisplayFor(model => model.Result).ToHtmlString();
                resultsDiv.InnerHtml += div.ToString(TagRenderMode.Normal);
            }
            wrapper.InnerHtml += resultsDiv.ToString(TagRenderMode.Normal);

            TagBuilder generalItemsTable = new TagBuilder("table");
            generalItemsTable.AddCssClass("availability-results-general-items");

            var remainingPairs = new TitleValuePair[]
            {
                new TitleValuePair()
                {
                    Title = helper.DisplayNameFor(model=> model.SVUID).ToHtmlString(),
                    Value = helper.DisplayFor(model=> model.SVUID).ToHtmlString()
                },
                new TitleValuePair()
                {
                    Title = helper.DisplayNameFor(model=> model.HasInfrastructure).ToHtmlString(),
                    Value = helper.DisplayFor(model=> model.HasInfrastructure).ToHtmlString()
                },
                new TitleValuePair()
                {
                    Title = helper.DisplayNameFor(model=> model.PortState).ToHtmlString(),
                    Value = helper.DisplayFor(model=> model.PortState).ToHtmlString()
                },
                new TitleValuePair()
                {
                    Title = helper.DisplayNameFor(model=> model.InfrastructureType).ToHtmlString(),
                    Value = helper.DisplayFor(model=> model.InfrastructureType).ToHtmlString()
                },
                new TitleValuePair()
                {
                    Title = helper.DisplayNameFor(model=> model.Distance).ToHtmlString(),
                    Value = helper.DisplayFor(model=> model.Distance).ToHtmlString()
                },
                new TitleValuePair()
                {
                    Title = helper.DisplayNameFor(model=> model.DistanceIsValid).ToHtmlString(),
                    Value = helper.DisplayFor(model=> model.DistanceIsValid).ToHtmlString()
                },
                new TitleValuePair()
                {
                    Title = helper.DisplayNameFor(model=> model.HasOpenRequest).ToHtmlString(),
                    Value = helper.DisplayFor(model=> model.HasOpenRequest).ToHtmlString()
                },
                new TitleValuePair()
                {
                    Title = helper.DisplayNameFor(model=> model.ErrorMessage).ToHtmlString(),
                    Value = helper.DisplayFor(model=> model.ErrorMessage).ToHtmlString(),
                    CSSClass = "error-message-value"
                }
            };

            foreach (var item in remainingPairs)
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("general-item");
                TagBuilder itemTitle = new TagBuilder("td");
                itemTitle.AddCssClass("general-title");
                itemTitle.InnerHtml = item.Title;
                row.InnerHtml += itemTitle.ToString(TagRenderMode.Normal);
                TagBuilder itemValue = new TagBuilder("td");
                itemValue.AddCssClass("general-value");
                if (item.CSSClass != null)
                {
                    itemValue.AddCssClass(item.CSSClass);
                }
                itemValue.InnerHtml = item.Value;
                row.InnerHtml += itemValue.ToString(TagRenderMode.Normal);
                generalItemsTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            wrapper.InnerHtml += generalItemsTable.ToString(TagRenderMode.Normal);



            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }

        class TitleValuePair
        {
            public string Title { get; set; }

            public string Value { get; set; }

            public string CSSClass { get; set; }
        }
    }
}