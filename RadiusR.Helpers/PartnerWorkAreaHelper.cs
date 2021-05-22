using RadiusR.Address;
using RadiusR_Manager.Models.RadiusViewModels;
using RezaB.Web.Helpers;
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
    public static class PartnerWorkAreaHelper
    {
        public static MvcHtmlString PartnerWorkAreaEditorFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : PartnerWorkAreaViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var Model = (PartnerWorkAreaViewModel)metadata.Model ?? new PartnerWorkAreaViewModel();
            var addressManager = new AddressManager();
            string serviceError = null;
            var Url = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);

            var oldPrefix = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
            if (string.IsNullOrEmpty(oldPrefix))
                helper.ViewData.TemplateInfo.HtmlFieldPrefix = fieldName;

            TagBuilder container = new TagBuilder("div");
            container.AddCssClass("address-editor-container");
            {
                TagBuilder table = new TagBuilder("table");
                {
                    TagBuilder row = new TagBuilder("tr");
                    row.AddCssClass("changing-list-row");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.ProvinceID).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");

                        var addressResults = addressManager.GetProvinces();
                        if (addressResults.ErrorOccured)
                        {
                            serviceError = addressResults.ErrorMessage;
                        }
                        else
                        {
                            cell.InnerHtml = helper.Select(model => Model.ProvinceID, new SelectList(addressResults.Data, "Code", "Name", Model.ProvinceID > 0 ? Model.ProvinceID : (long?)null), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                            cell.InnerHtml += helper.HiddenFor(model => Model.ProvinceName, new { @class = "address-name" });
                            TagBuilder checkmark = new TagBuilder("div");
                            checkmark.AddCssClass("address-checkmark");
                            cell.InnerHtml += checkmark.ToString(TagRenderMode.Normal);
                        }
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    //---------load next link-------
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("style", "display: none;");
                        cell.SetInnerText(Url.Action("GetProvinceDistricts", "Address"));
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                if (serviceError == null)
                {
                    TagBuilder row = new TagBuilder("tr");
                    row.AddCssClass("changing-list-row");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.DistrictID).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        if (Model.ProvinceID > 0)
                        {
                            var addressResults = addressManager.GetProvinceDistricts(Model.ProvinceID);
                            if (addressResults.ErrorOccured)
                            {
                                serviceError = addressResults.ErrorMessage;
                            }
                            else
                            {
                                cell.InnerHtml = helper.Select(model => Model.DistrictID, new SelectList(addressResults.Data, "Code", "Name", Model.DistrictID.HasValue ? Model.DistrictID : (long?)null), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                            }
                        }
                        else
                        {
                            cell.InnerHtml = helper.Select(model => Model.DistrictID, new SelectList(Enumerable.Empty<object>()), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                        }
                        cell.InnerHtml += helper.HiddenFor(model => Model.DistrictName, new { @class = "address-name" });
                        TagBuilder checkmark = new TagBuilder("div");
                        checkmark.AddCssClass("address-checkmark");
                        cell.InnerHtml += checkmark.ToString(TagRenderMode.Normal);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    //---------load next link-------
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("style", "display: none;");
                        cell.SetInnerText(Url.Action("GetDistrictRuralRegions", "Address"));
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                if (serviceError == null)
                {
                    TagBuilder row = new TagBuilder("tr");
                    row.AddCssClass("changing-list-row");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.RuralCode).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        if (Model.DistrictID.HasValue)
                        {
                            var addressResults = addressManager.GetDistrictRuralRegions(Model.DistrictID.Value);
                            if (addressResults.ErrorOccured)
                            {
                                serviceError = addressResults.ErrorMessage;
                            }
                            else
                            {
                                cell.InnerHtml = helper.Select(model => Model.RuralCode, new SelectList(addressResults.Data, "Code", "Name", Model.RuralCode.HasValue ? Model.RuralCode : (long?)null), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                            }
                        }
                        else
                        {
                            cell.InnerHtml = helper.Select(model => Model.RuralCode, new SelectList(Enumerable.Empty<object>()), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                        }
                        TagBuilder checkmark = new TagBuilder("div");
                        checkmark.AddCssClass("address-checkmark");
                        cell.InnerHtml += checkmark.ToString(TagRenderMode.Normal);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    //---------load next link-------
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("style", "display: none;");
                        cell.SetInnerText(Url.Action("GetRuralRegionNeighbourhoods", "Address"));
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                if (serviceError == null)
                {
                    TagBuilder row = new TagBuilder("tr");
                    row.AddCssClass("changing-list-row");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.NeighbourhoodID).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        if (Model.RuralCode.HasValue)
                        {
                            var addressResults = addressManager.GetRuralRegionNeighbourhoods(Model.RuralCode.Value);
                            if (addressResults.ErrorOccured)
                            {
                                serviceError = addressResults.ErrorMessage;
                            }
                            else
                            {
                                cell.InnerHtml = helper.Select(model => Model.NeighbourhoodID, new SelectList(addressResults.Data, "Code", "Name", Model.NeighbourhoodID.HasValue ? Model.NeighbourhoodID : (long?)null), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                            }
                        }
                        else
                        {
                            cell.InnerHtml = helper.Select(model => Model.NeighbourhoodID, new SelectList(Enumerable.Empty<object>()), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                        }
                        cell.InnerHtml += helper.HiddenFor(model => Model.NeighbourhoodName, new { @class = "address-name" });
                        TagBuilder checkmark = new TagBuilder("div");
                        checkmark.AddCssClass("address-checkmark");
                        cell.InnerHtml += checkmark.ToString(TagRenderMode.Normal);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    //---------load next link-------
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("style", "display: none;");
                        cell.SetInnerText(Url.Action("GetNeighbourhoodStreets", "Address"));
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }

                container.InnerHtml = table.ToString(TagRenderMode.Normal);
            }

            if (serviceError != null)
            {
                TagBuilder errorMessage = new TagBuilder("div");
                errorMessage.AddCssClass("text-danger");
                errorMessage.SetInnerText(serviceError);
                container.InnerHtml = errorMessage.ToString(TagRenderMode.Normal);
            }

            helper.ViewData.TemplateInfo.HtmlFieldPrefix = oldPrefix;

            return new MvcHtmlString(container.ToString(TagRenderMode.Normal));

        }

        public static MvcHtmlString PartnerWorkAreaDisplayFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : PartnerWorkAreaViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var Model = (PartnerWorkAreaViewModel)metadata.Model ?? new PartnerWorkAreaViewModel();

            TagBuilder container = new TagBuilder("span");
            container.AddCssClass("work-area-container");

            container.InnerHtml = Model.ProvinceName;
            if (!string.IsNullOrWhiteSpace(Model.DistrictName))
            {
                container.InnerHtml += "&nbsp;<span>&gt;</span>&nbsp;" + Model.DistrictName;
            }
            if (!string.IsNullOrWhiteSpace(Model.NeighbourhoodName))
            {
                container.InnerHtml += "&nbsp;<span>&gt;</span>&nbsp;" + Model.NeighbourhoodName;
            }

            return new MvcHtmlString(container.ToString(TagRenderMode.Normal));
        }
    }
}
