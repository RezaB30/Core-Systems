using RadiusR.DB.Enums;
using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RezaB.Web.Helpers;
using RezaB.Data.Localization;

namespace RadiusR_Manager.Helpers
{
    public static class ClientFeeListHelper
    {
        public static MvcHtmlString ClientFeeList<TModel, Tresult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, Tresult>> expression) where TModel : IEnumerable<ClientFeeViewModel>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = ((IEnumerable<ClientFeeViewModel>)metadata.Model).ToArray();


            string rows = "";
            for (int i = 0; i < model.Length; i++)
            {
                var prefix = fullName + "[" + i + "].";
                var clientFee = model[i];
                TagBuilder row = new TagBuilder("tr");

                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    cell.AddCssClass("additional-fee-cell");

                    TagBuilder feeTypeIDHidden = new TagBuilder("input");
                    feeTypeIDHidden.MergeAttribute("type", "hidden");
                    feeTypeIDHidden.MergeAttribute("name", prefix + "FeeTypeID");
                    feeTypeIDHidden.MergeAttribute("value", clientFee.FeeTypeID.ToString());

                    cell.InnerHtml += feeTypeIDHidden.ToString(TagRenderMode.Normal);
                    if (clientFee.FeeType._price.HasValue)
                    {
                        cell.InnerHtml += helper.CheckButton(prefix + "IsChecked", new LocalizedList<FeeType, RadiusR.Localization.Lists.FeeType>().GetDisplayText(clientFee.FeeTypeID), clientFee.IsChecked);
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    else
                    {
                        TagBuilder checkbox = new TagBuilder("input");
                        checkbox.MergeAttribute("type", "checkbox");
                        checkbox.MergeAttribute("checked", "checked");
                        checkbox.MergeAttribute("style", "display: none;");
                        checkbox.MergeAttribute("name", prefix + "IsChecked");
                        cell.InnerHtml += checkbox.ToString(TagRenderMode.SelfClosing);

                        TagBuilder div = new TagBuilder("div");
                        div.AddCssClass("fee-type-variant-container");
                        div.InnerHtml += helper.DisplayFor(modelItem => clientFee.FeeTypeID);
                        div.InnerHtml += helper.Select(prefix + "FeeTypeVariantID", new SelectList(clientFee.FeeType.FeeTypeVariants.ToList(), "ID", "Title", clientFee.FeeTypeVariantID), RadiusR.Localization.Pages.Common.Choose);
                        cell.InnerHtml += div.ToString(TagRenderMode.Normal);

                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                }
                if (clientFee.IsAllTime)
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");

                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                else
                {
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml += helper.DisplayNameFor(modelItem => clientFee.InstallmentCount); 

                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml += helper.Select(prefix + "InstallmentCount", HelperUtilities.CreateNumericSelectList(1, 24, clientFee.InstallmentCount));

                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                }

                rows += row.ToString(TagRenderMode.Normal);
            }

            return new MvcHtmlString(rows);
        }

        public static MvcHtmlString ClientFeeListDisplay<TModel, Tresult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, Tresult>> expression) where TModel : IEnumerable<ClientFeeViewModel>
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = ((IEnumerable<ClientFeeViewModel>)metadata.Model).ToArray();


            string rows = "";
            for (int i = 0; i < model.Length; i++)
            {
                var prefix = fullName + "[" + i + "].";
                var clientFee = model[i];
                TagBuilder row = new TagBuilder("tr");

                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");
                    //cell.AddCssClass("additional-fee-cell");

                    TagBuilder feeTypeIDHidden = new TagBuilder("input");
                    feeTypeIDHidden.MergeAttribute("type", "hidden");
                    feeTypeIDHidden.MergeAttribute("name", prefix + "FeeTypeID");
                    feeTypeIDHidden.MergeAttribute("value", clientFee.FeeTypeID.ToString());

                    cell.InnerHtml += feeTypeIDHidden.ToString(TagRenderMode.Normal);
                    if (clientFee.FeeType._price.HasValue)
                    {
                        TagBuilder selectedHidden = new TagBuilder("input");
                        selectedHidden.MergeAttribute("name", "IsChecked");
                        selectedHidden.MergeAttribute("style", "display: none;");
                        selectedHidden.MergeAttribute("checked", (clientFee.IsChecked) ? "checked" : "");
                        selectedHidden.MergeAttribute("type", "checkbox");

                        cell.InnerHtml += selectedHidden.ToString(TagRenderMode.SelfClosing);
                        cell.InnerHtml += helper.DisplayFor(modelItem => clientFee.IsChecked);
                        cell.InnerHtml += helper.DisplayFor(modelItem => clientFee.FeeTypeID);

                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    else
                    {
                        TagBuilder checkbox = new TagBuilder("input");
                        checkbox.MergeAttribute("type", "checkbox");
                        checkbox.MergeAttribute("checked", "checked");
                        checkbox.MergeAttribute("style", "display: none;");
                        checkbox.MergeAttribute("name", prefix + "IsChecked");

                        cell.InnerHtml += checkbox.ToString(TagRenderMode.SelfClosing);
                        cell.InnerHtml += helper.DisplayFor(modelItem => clientFee.IsChecked);
                        cell.InnerHtml += helper.DisplayFor(modelItem => clientFee.FeeTypeID);
                        var feeTypeVariant = clientFee.FeeType.FeeTypeVariants.FirstOrDefault(variant => variant.ID == clientFee.FeeTypeVariantID);
                        cell.InnerHtml += helper.DisplayFor(modelItem => feeTypeVariant.Title);
                        cell.InnerHtml += helper.Hidden(prefix + "FeeTypeVariantID", clientFee.FeeTypeVariantID);

                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                }
                if (clientFee.IsAllTime)
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("colspan", "2");

                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                else
                {
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml += helper.LabelFor(modelItem => clientFee.InstallmentCount);

                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml += helper.DisplayFor(modelItem => clientFee.InstallmentCount);
                        cell.InnerHtml += helper.Hidden(prefix + "InstallmentCount", clientFee.InstallmentCount);

                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                }

                rows += row.ToString(TagRenderMode.Normal);
            }

            return new MvcHtmlString(rows);
        }
    }
}