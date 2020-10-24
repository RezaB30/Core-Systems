using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using RezaB.Web.Helpers;
using RezaB.Data.Localization;

namespace RadiusR_Manager.Helpers
{
    public static class PaymentButtonHelper
    {
        public static MvcHtmlString PaymentButton<TModel>(this HtmlHelper<TModel> helper, Expression<Func<TModel, short>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = (short)metadata.Model;


            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("payment-button-combo-wrapper");

            TagBuilder table = new TagBuilder("table");
            TagBuilder row = new TagBuilder("tr");
            {
                TagBuilder cell = new TagBuilder("td");

                TagBuilder label = new TagBuilder("label");
                label.MergeAttribute("for", fullName);

                label.InnerHtml += RadiusR.Localization.Model.RadiusR.PaymentType;

                cell.InnerHtml += label.ToString(TagRenderMode.Normal);
                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder cell = new TagBuilder("td");

                var validPaymentTypes = new int[] { (int)PaymentType.Cash, (int)PaymentType.None, (int)PaymentType.PhysicalPos, (int)PaymentType.Transfer, (int)PaymentType.VirtualPos };
                var localizedListItems = new LocalizedList<PaymentType, RadiusR.Localization.Lists.PaymentType>().GenericList.Where(item => validPaymentTypes.Contains(item.ID));
                cell.InnerHtml += helper.Select(fullName, new SelectList(localizedListItems, "ID", "Name", model));
                cell.InnerHtml += "&nbsp;";

                TagBuilder payButton = new TagBuilder("input");
                payButton.MergeAttribute("type", "submit");
                payButton.MergeAttribute("value", RadiusR.Localization.Pages.Common.Pay);
                payButton.MergeAttribute("style", "display: none;");
                payButton.AddCssClass("link-button");
                payButton.AddCssClass("iconed-button");
                payButton.AddCssClass("payment-button");

                cell.InnerHtml += payButton.ToString(TagRenderMode.SelfClosing);
                cell.InnerHtml += "&nbsp;";

                TagBuilder payAndPrintButton = new TagBuilder("input");
                payAndPrintButton.MergeAttribute("type", "button");
                payAndPrintButton.MergeAttribute("value", RadiusR.Localization.Pages.Common.PayAndPrint);
                payAndPrintButton.MergeAttribute("style", "display: none;");
                payAndPrintButton.AddCssClass("link-button");
                payAndPrintButton.AddCssClass("iconed-button");
                payAndPrintButton.AddCssClass("print-button");

                cell.InnerHtml += payAndPrintButton.ToString(TagRenderMode.SelfClosing);
                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
            }

            table.InnerHtml += row.ToString(TagRenderMode.Normal);
            wrapper.InnerHtml += table.ToString(TagRenderMode.Normal);


            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}