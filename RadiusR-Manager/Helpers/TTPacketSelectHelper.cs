using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RezaB.Web.Helpers;

namespace RadiusR_Manager.Helpers
{
    public static class TTPacketSelectHelper
    {
        public static MvcHtmlString TTPacketSelectFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression) where TModel : TTPacketViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var value = metadata.Model;

            TagBuilder table = new TagBuilder("table");
            table.AddCssClass("input-table");
            table.AddCssClass("telekom-packet-selector-container");
            {
                TagBuilder row = new TagBuilder("tr");
                TagBuilder cell = new TagBuilder("td");
                cell.MergeAttribute("style", "display: none;");
                cell.InnerHtml += helper.HiddenFor(model => model.Speed, new { @class = "speed-hidden" });
                cell.InnerHtml += helper.HiddenFor(model => model.TariffCode, new { @class = "tariff-code-hidden" });
                cell.InnerHtml += helper.HiddenFor(model => model.PacketCode, new { @class = "packet-code-hidden" });
                row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.LabelFor(model => model.DSLType);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.EditorFor(model => model.DSLType, new { @htmlAttributes = new { @class = "ttpacket-dsl-type-selector" } });
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                row.MergeAttribute("style", "display: none;");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.MergeAttribute("rowspan", "2");
                    cell.InnerHtml += helper.LabelFor(model => model.PacketCode);
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.Select("", new SelectList(Enumerable.Empty<object>()), RadiusR.Localization.Pages.Common.Choose, htmlAttributes: new { @class = "ttpacket-tariff-selector" });
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }
            {
                TagBuilder row = new TagBuilder("tr");
                row.MergeAttribute("style", "display: none;");
                {
                    TagBuilder cell = new TagBuilder("td");
                    cell.InnerHtml += helper.Select("", new SelectList(Enumerable.Empty<object>()), RadiusR.Localization.Pages.Common.Choose, htmlAttributes: new { @class = "ttpacket-packet-selector" });
                    row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                }
                table.InnerHtml += row.ToString(TagRenderMode.Normal);
            }

            return new MvcHtmlString(table.ToString(TagRenderMode.Normal));
        }
    }
}