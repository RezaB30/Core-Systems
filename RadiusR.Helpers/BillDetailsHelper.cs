using RadiusR.DB.Enums;
using RadiusR_Manager.Models.RadiusViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace RadiusR.Helpers
{
    public static class BillDetailsHelper
    {
        public static MvcHtmlString BillDetailsFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TResult : BillViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var Model = (BillViewModel)metadata.Model;

            TagBuilder mainTable = new TagBuilder("table");
            mainTable.AddCssClass("bill-receipt");
            // head rows
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("receipt-head-row");
                {
                    TagBuilder head = new TagBuilder("th");
                    head.MergeAttribute("style", "text-align: left;");
                    head.InnerHtml = helper.DisplayFor(model => Model.Subscription.Name).ToHtmlString();
                    row.InnerHtml += head.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder head = new TagBuilder("th");
                    head.MergeAttribute("style", "text-align: right; border-left: none;");
                    head.InnerHtml = helper.DisplayFor(model => Model.Source).ToHtmlString();
                    row.InnerHtml += head.ToString(TagRenderMode.Normal);
                }
                mainTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("receipt-head-row");
                {
                    TagBuilder head = new TagBuilder("th");
                    head.MergeAttribute("style", "text-align: left;");
                    head.InnerHtml = helper.DisplayNameFor(model => Model.IssueDate).ToHtmlString();
                    row.InnerHtml += head.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder head = new TagBuilder("th");
                    head.MergeAttribute("style", "text-align: right; border-left: none;");
                    head.InnerHtml = helper.DisplayFor(model => Model.IssueDate).ToHtmlString();
                    row.InnerHtml += head.ToString(TagRenderMode.Normal);
                }
                mainTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("receipt-head-row");
                {
                    TagBuilder head = new TagBuilder("th");
                    head.MergeAttribute("style", "text-align: left;");
                    head.InnerHtml = helper.DisplayNameFor(model => Model.DueDate).ToHtmlString();
                    row.InnerHtml += head.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder head = new TagBuilder("th");
                    head.MergeAttribute("style", "text-align: right; border-left: none;");
                    head.InnerHtml = helper.DisplayFor(model => Model.DueDate).ToHtmlString();
                    row.InnerHtml += head.ToString(TagRenderMode.Normal);
                }
                mainTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            if (Model.PeriodStart.HasValue && Model.PeriodEnd.HasValue)
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("receipt-head-row");
                {
                    TagBuilder head = new TagBuilder("th");
                    head.MergeAttribute("colspan", "2");
                    head.MergeAttribute("style", "text-align: left;");
                    head.InnerHtml = helper.DisplayNameFor(model => Model.PeriodStart).ToHtmlString();
                    head.InnerHtml += " (" + helper.DisplayFor(model => Model.PeriodStart).ToHtmlString() + "-" + helper.DisplayFor(model => Model.PeriodEnd) + ")";
                    row.InnerHtml += head.ToString(TagRenderMode.Normal);
                }
                mainTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("receipt-head-row");
                {
                    TagBuilder head = new TagBuilder("th");
                    head.InnerHtml = Localization.Pages.Common.Description;
                    row.InnerHtml += head.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder head = new TagBuilder("th");
                    head.InnerHtml = new HtmlString(Localization.Model.RadiusR.Price + "/" + Localization.Pages.Common.Discount).ToHtmlString();
                    row.InnerHtml += head.ToString(TagRenderMode.Normal);
                }
                mainTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            // fees
            foreach (var billFee in Model.BillFees)
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += new HtmlString(billFee.DisplayName).ToHtmlString();
                    if (billFee.StartDate.HasValue && billFee.EndDate.HasValue)
                    {
                        TagBuilder span = new TagBuilder("span");
                        span.AddCssClass("fee-time-span");
                        span.InnerHtml = "(" + helper.DisplayFor(model => billFee.StartDate).ToHtmlString() + "-" + helper.DisplayFor(model => billFee.EndDate).ToHtmlString() + ")";
                        cell.InnerHtml += " " + span.ToString(TagRenderMode.Normal);
                    }
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.DisplayFor(model => billFee.CurrentCost).ToHtmlString();
                    if (billFee._discountAmount.HasValue)
                        cell.InnerHtml += " - " + helper.DisplayFor(model => billFee.DiscountAmount).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                mainTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            // total row
            {
                TagBuilder row = new TagBuilder("tr");
                row.AddCssClass("total-row");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.DisplayNameFor(model => Model.TotalPayableAmount).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml = helper.DisplayFor(model => Model.TotalPayableAmount).ToHtmlString();
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                mainTable.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            // e-bill details row
            if (Model.EBill != null)
            {
                TagBuilder eBillRow = new TagBuilder("tr");
                eBillRow.AddCssClass("ebill-row");
                TagBuilder eBillCell = new TagBuilder("td");
                eBillCell.MergeAttribute("colspan", "2");

                TagBuilder eBillTable = new TagBuilder("table");
                eBillTable.AddCssClass("ebill-table");
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.DisplayNameFor(model => Model.EBill.ReferenceNo).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "3");
                        cell.AddCssClass("value-cell");
                        cell.InnerHtml = helper.DisplayFor(model => Model.EBill.ReferenceNo).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    eBillTable.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.DisplayNameFor(model => Model.EBill.BillCode).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.AddCssClass("value-cell");
                        cell.InnerHtml = helper.DisplayFor(model => Model.EBill.BillCode).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.DisplayNameFor(model => Model.EBill.EBillIssueDate).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.AddCssClass("value-cell");
                        cell.InnerHtml = helper.DisplayFor(model => Model.EBill.EBillIssueDate).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    eBillTable.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.DisplayNameFor(model => Model.EBill.Date).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.AddCssClass("value-cell");
                        cell.InnerHtml = helper.DisplayFor(model => Model.EBill.Date).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.DisplayNameFor(model => Model.EBill.EBillType).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.AddCssClass("value-cell");
                        cell.InnerHtml = helper.DisplayFor(model => Model.EBill.EBillType).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    eBillTable.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                eBillCell.InnerHtml = eBillTable.ToString(TagRenderMode.Normal);
                eBillRow.InnerHtml = eBillCell.ToString(TagRenderMode.Normal);
                mainTable.InnerHtml += eBillRow.ToString(TagRenderMode.Normal);
            }

            return new MvcHtmlString(mainTable.ToString(TagRenderMode.Normal));
        }
    }
}
