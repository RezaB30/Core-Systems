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
using RezaB.TurkTelekom.WebServices.Address;
using RadiusR.Address;

namespace RadiusR.Helpers
{
    public static class AddressHelper
    {
        public static MvcHtmlString AddressDetailsFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : AddressViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var Model = (AddressViewModel)metadata.Model;

            TagBuilder ContainerDiv = new TagBuilder("div");
            ContainerDiv.AddCssClass("address-details-container");
            ContainerDiv.AddCssClass("closed");

            {
                TagBuilder ToggleButton = new TagBuilder("div");
                ToggleButton.AddCssClass("address-details-toggle");
                ToggleButton.MergeAttribute("tab-index", "0");
                ContainerDiv.InnerHtml = ToggleButton.ToString(TagRenderMode.Normal);
            }


            {
                TagBuilder table = new TagBuilder("table");
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.ProvinceID).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("code-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.ProvinceID).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        cell.InnerHtml += " - ";
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("text-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.ProvinceName).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.DistrictID).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("code-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.DistrictID).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        cell.InnerHtml += " - ";
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("text-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.DistrictName).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.NeighbourhoodID).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("code-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.NeighbourhoodID).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        cell.InnerHtml += " - ";
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("text-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.NeighborhoodName).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.StreetID).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("code-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.StreetID).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        cell.InnerHtml += " - ";
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("text-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.StreetName).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.DoorID).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("code-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.DoorID).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        cell.InnerHtml += " - ";
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("text-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.DoorNo).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.ApartmentID).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("code-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.ApartmentID).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        cell.InnerHtml += " - ";
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("text-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.ApartmentNo).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    row.AddCssClass("all-time-visible");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.AddressText).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("code-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.AddressNo).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        cell.InnerHtml += " - ";
                        {
                            TagBuilder span = new TagBuilder("span");
                            span.AddCssClass("text-part");
                            span.InnerHtml = helper.DisplayFor(model => Model.AddressText).ToHtmlString();
                            cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        }
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.PostalCode).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.DisplayFor(model => Model.PostalCode).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.Floor).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.DisplayFor(model => Model.Floor).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                ContainerDiv.InnerHtml += table.ToString(TagRenderMode.Normal);
            }

            return new MvcHtmlString(ContainerDiv.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString AddressEditorFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : AddressViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var Model = (AddressViewModel)metadata.Model ?? new AddressViewModel();
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
                //----------------- BBK input -------------------
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.Label(string.Empty, RadiusR.Localization.Helpers.Common.BBK).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        TagBuilder bbkInput = new TagBuilder("input");
                        bbkInput.MergeAttributes(new Dictionary<string, string>()
                            {
                                { "maxlength", "10" },
                                { "autocomplete", "off" }
                            });
                        bbkInput.AddCssClass("bbk-input");
                        cell.InnerHtml = bbkInput.ToString(TagRenderMode.SelfClosing);
                        TagBuilder fetchButton = new TagBuilder("input");
                        fetchButton.MergeAttribute("type", "button");
                        fetchButton.MergeAttribute("style", "vertical-align: middle;");
                        fetchButton.AddCssClass("bbk-fetch-button link-button iconed-button accept-button");
                        cell.InnerHtml += "&nbsp;" + fetchButton.ToString(TagRenderMode.SelfClosing);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("style", "display: none;");
                        cell.SetInnerText(Url.Action("GetApartmentAddress", "Address"));
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "2");
                        cell.AddCssClass("text-danger bbk-validation-error");
                        row.InnerHtml = cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "2");
                        cell.AddCssClass("divider-cell");
                        cell.InnerHtml = RadiusR.Localization.Helpers.Common.Or;
                        row.InnerHtml = cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                //-----------------------------------------------
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
                                cell.InnerHtml = helper.Select(model => Model.DistrictID, new SelectList(addressResults.Data, "Code", "Name", Model.DistrictID > 0 ? Model.DistrictID : (long?)null), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
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
                        if (Model.DistrictID > 0)
                        {
                            var addressResults = addressManager.GetDistrictRuralRegions(Model.DistrictID);
                            if (addressResults.ErrorOccured)
                            {
                                serviceError = addressResults.ErrorMessage;
                            }
                            else
                            {
                                cell.InnerHtml = helper.Select(model => Model.RuralCode, new SelectList(addressResults.Data, "Code", "Name", Model.RuralCode > 0 ? Model.RuralCode : (long?)null), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
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
                        if (Model.RuralCode > 0)
                        {
                            var addressResults = addressManager.GetRuralRegionNeighbourhoods(Model.RuralCode);
                            if (addressResults.ErrorOccured)
                            {
                                serviceError = addressResults.ErrorMessage;
                            }
                            else
                            {
                                cell.InnerHtml = helper.Select(model => Model.NeighbourhoodID, new SelectList(addressResults.Data, "Code", "Name", Model.NeighbourhoodID > 0 ? Model.NeighbourhoodID : (long?)null), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                            }
                        }
                        else
                        {
                            cell.InnerHtml = helper.Select(model => Model.NeighbourhoodID, new SelectList(Enumerable.Empty<object>()), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                        }
                        cell.InnerHtml += helper.HiddenFor(model => Model.NeighborhoodName, new { @class = "address-name" });
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
                if (serviceError == null)
                {
                    TagBuilder row = new TagBuilder("tr");
                    row.AddCssClass("changing-list-row");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.StreetID).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        if (Model.NeighbourhoodID > 0)
                        {
                            var addressResults = addressManager.GetNeighbourhoodStreets(Model.NeighbourhoodID);
                            if (addressResults.ErrorOccured)
                            {
                                serviceError = addressResults.ErrorMessage;
                            }
                            else
                            {
                                cell.InnerHtml = helper.Select(model => Model.StreetID, new SelectList(addressResults.Data, "Code", "Name", Model.StreetID > 0 ? Model.StreetID : (long?)null), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                            }
                        }
                        else
                        {
                            cell.InnerHtml = helper.Select(model => Model.StreetID, new SelectList(Enumerable.Empty<object>()), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                        }
                        cell.InnerHtml += helper.HiddenFor(model => Model.StreetName, new { @class = "address-name" });
                        TagBuilder checkmark = new TagBuilder("div");
                        checkmark.AddCssClass("address-checkmark");
                        cell.InnerHtml += checkmark.ToString(TagRenderMode.Normal);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    //---------load next link-------
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("style", "display: none;");
                        cell.SetInnerText(Url.Action("GetStreetBuildings", "Address"));
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
                        cell.InnerHtml = helper.LabelFor(model => Model.DoorID).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        if (Model.StreetID > 0)
                        {
                            var addressResults = addressManager.GetStreetBuildings(Model.StreetID);
                            if (addressResults.ErrorOccured)
                            {
                                serviceError = addressResults.ErrorMessage;
                            }
                            else
                            {
                                cell.InnerHtml = helper.Select(model => Model.DoorID, new SelectList(addressResults.Data, "Code", "Name", Model.DoorID > 0 ? Model.DoorID : (long?)null), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                            }
                        }
                        else
                        {
                            cell.InnerHtml = helper.Select(model => Model.DoorID, new SelectList(Enumerable.Empty<object>()), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                        }
                        cell.InnerHtml += helper.HiddenFor(model => Model.DoorNo, new { @class = "address-name" });
                        TagBuilder checkmark = new TagBuilder("div");
                        checkmark.AddCssClass("address-checkmark");
                        cell.InnerHtml += checkmark.ToString(TagRenderMode.Normal);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    //---------load next link-------
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("style", "display: none;");
                        cell.SetInnerText(Url.Action("GetBuildingApartments", "Address"));
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                if (serviceError == null)
                {
                    TagBuilder row = new TagBuilder("tr");
                    row.AddCssClass("address-result-change");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.ApartmentID).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        if (Model.DoorID > 0)
                        {
                            var addressResults = addressManager.GetBuildingApartments(Model.DoorID);
                            if (addressResults.ErrorOccured)
                            {
                                serviceError = addressResults.ErrorMessage;
                            }
                            else
                            {
                                cell.InnerHtml = helper.Select(model => Model.ApartmentID, new SelectList(addressResults.Data, "Code", "Name", Model.ApartmentID != 0 ? Model.ApartmentID : (long?)null), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                            }
                        }
                        else
                        {
                            cell.InnerHtml = helper.Select(model => Model.ApartmentID, new SelectList(Enumerable.Empty<object>()), Localization.Pages.Common.Choose, null, new { @class = "address-code" }).ToHtmlString();
                        }
                        cell.InnerHtml += helper.HiddenFor(model => Model.ApartmentNo, new { @class = "address-name" });
                        TagBuilder checkmark = new TagBuilder("div");
                        checkmark.AddCssClass("address-checkmark");
                        cell.InnerHtml += checkmark.ToString(TagRenderMode.Normal);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    //---------load next link-------
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("style", "display: none;");
                        cell.SetInnerText(Url.Action("GetApartmentAddress", "Address"));
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                // address code display
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        TagBuilder label = new TagBuilder("label");
                        label.SetInnerText(Localization.Model.RadiusR.BBK);
                        cell.InnerHtml = label.ToString(TagRenderMode.Normal);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.AddCssClass("address-code-display");
                        cell.SetInnerText(Model.ApartmentID.ToString());
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                // address text
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.AddressText).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml += helper.HiddenFor(model => Model.AddressText, new { @class = "address-text-input" }).ToHtmlString();
                        cell.InnerHtml += helper.HiddenFor(model => Model.AddressNo, new { @class = "address-no-input" }).ToHtmlString();
                        TagBuilder span = new TagBuilder("span");
                        span.AddCssClass("address-text-display");
                        span.InnerHtml += helper.DisplayFor(model => Model.AddressText).ToHtmlString();
                        cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                // address number
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.AddressNo).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml += helper.HiddenFor(model => Model.AddressNo, new { @class = "address-no-input" }).ToHtmlString();
                        TagBuilder span = new TagBuilder("span");
                        span.AddCssClass("address-no-display");
                        span.InnerHtml += helper.DisplayFor(model => Model.AddressNo).ToHtmlString();
                        cell.InnerHtml += span.ToString(TagRenderMode.Normal);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                // postal code
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.PostalCode).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml += helper.TextBoxFor(model => Model.PostalCode, new { @maxlength = 5, @autocomplete = "off" });
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.PostalCode).ToHtmlString();
                    row.InnerHtml = cell.ToString(TagRenderMode.Normal);
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    //cell.InnerHtml = helper.ValidationMessageFor(model => Model.AddressNo).ToHtmlString();
                    row.InnerHtml = cell.ToString(TagRenderMode.Normal);
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                // floor
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(model => Model.Floor).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml += helper.TextBoxFor(model => Model.Floor, new { @maxlength = 150, @autocomplete = "off" });
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.Floor).ToHtmlString();
                    row.InnerHtml = cell.ToString(TagRenderMode.Normal);
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.InnerHtml = helper.ValidationMessageFor(model => Model.AddressNo).ToHtmlString();
                    row.InnerHtml = cell.ToString(TagRenderMode.Normal);
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

        public static MvcHtmlString AddressHiddenFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : AddressViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var Model = (AddressViewModel)metadata.Model;

            StringBuilder results = new StringBuilder("");
            var oldPrefix = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
            if (string.IsNullOrEmpty(oldPrefix))
                helper.ViewData.TemplateInfo.HtmlFieldPrefix = fieldName;
            results.Append(helper.HiddenFor(model => Model.AddressNo));
            results.Append(helper.HiddenFor(model => Model.AddressText));
            results.Append(helper.HiddenFor(model => Model.ApartmentID));
            results.Append(helper.HiddenFor(model => Model.ApartmentNo));
            results.Append(helper.HiddenFor(model => Model.DistrictID));
            results.Append(helper.HiddenFor(model => Model.DistrictName));
            results.Append(helper.HiddenFor(model => Model.DoorID));
            results.Append(helper.HiddenFor(model => Model.DoorNo));
            results.Append(helper.HiddenFor(model => Model.NeighbourhoodID));
            results.Append(helper.HiddenFor(model => Model.NeighborhoodName));
            results.Append(helper.HiddenFor(model => Model.PostalCode));
            results.Append(helper.HiddenFor(model => Model.Floor));
            results.Append(helper.HiddenFor(model => Model.ProvinceID));
            results.Append(helper.HiddenFor(model => Model.ProvinceName));
            results.Append(helper.HiddenFor(model => Model.RuralCode));
            results.Append(helper.HiddenFor(model => Model.StreetID));
            results.Append(helper.HiddenFor(model => Model.StreetName));
            helper.ViewData.TemplateInfo.HtmlFieldPrefix = oldPrefix;

            return new MvcHtmlString(results.ToString());
        }
    }
}
