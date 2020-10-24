using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using RadiusR_Manager.Models.ViewModels;
using RadiusR.DB.DomainsCache;
using System.Web.Script.Serialization;
using System.Web.Mvc.Html;
using RezaB.Web.Helpers;
using RezaB.TurkTelekom.WebServices;

namespace RadiusR.Helpers
{
    public static class TelekomTariffHelper
    {
        public static MvcHtmlString TelekomTariffEditorFor<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, CachedDomain domain) where TResult : TelekomTariffHelperViewModel
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
            var model = metadata.Model as TelekomTariffHelperViewModel ?? new TelekomTariffHelperViewModel();

            if (domain == null)
                return new MvcHtmlString(RadiusR.Localization.Helpers.Common.NoData);

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("telekom-tariff-editor-wrapper");

            IEnumerable<object> speedList = Enumerable.Empty<object>();
            IEnumerable<object> speedTariffsList = Enumerable.Empty<object>();
            // construct json
            {
                var allTariffs = TelekomTariffsCache.GetAllTariffs(domain);
                if (allTariffs == null || !allTariffs.Any())
                    return new MvcHtmlString(RadiusR.Localization.Helpers.Common.NoData);
                var groupedTariffs = allTariffs.GroupBy(t => t.XDSLType).Select(tg => new
                {
                    InfrastructureType = tg.Key,
                    Tariffs = tg.GroupBy(tgg => new { SpeedCode = tgg.SpeedCode, SpeedName = tgg.SpeedName }).Select(tgg => new
                    {
                        Speed = tgg.Key,
                        Tariffs = tgg.Select(tggvm => new TelekomTariffHelperViewModel()
                        {
                            MonthlyStaticFee = tggvm.MonthlyStaticFee,
                            PacketCode = tggvm.PacketCode,
                            SpeedCode = tggvm.SpeedCode,
                            SpeedDetails = tggvm.SpeedDetails,
                            SpeedName = tggvm.SpeedName,
                            TariffCode = tggvm.TariffCode,
                            TariffName = tggvm.TariffName,
                            XDSLType = (short)tggvm.XDSLType
                        }).ToArray()
                    })
                }).ToArray();

                TagBuilder jsonSource = new TagBuilder("div");
                jsonSource.MergeAttribute("style", "display: none;");
                jsonSource.AddCssClass("json-source");

                var serializer = new JavaScriptSerializer();
                jsonSource.SetInnerText(serializer.Serialize(groupedTariffs));

                wrapper.InnerHtml += jsonSource.ToString(TagRenderMode.Normal);

                if (model.XDSLType.HasValue)
                {
                    speedList = groupedTariffs.FirstOrDefault(gt => gt.InfrastructureType == (XDSLType)model.XDSLType.Value).Tariffs.Select(t => new { Name = t.Speed.SpeedName, Value = t.Speed.SpeedCode }).ToArray();
                    if (model.SpeedCode.HasValue)
                    {
                        speedTariffsList = groupedTariffs.FirstOrDefault(gt => gt.InfrastructureType == (XDSLType)model.XDSLType.Value).Tariffs.FirstOrDefault(gt => gt.Speed.SpeedCode == model.SpeedCode).Tariffs.Select(t => new { Value = t.TariffCode + "," + t.PacketCode, Name = t.TariffName }).ToArray();
                    }
                }
            }

            var oldPrefix = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
            helper.ViewData.TemplateInfo.HtmlFieldPrefix = fullName;

            // input table
            {
                TagBuilder table = new TagBuilder("table");
                table.AddCssClass("input-table");

                // xdsl type row
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(modelItem => model.XDSLType).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.EditorFor(modelItem => model.XDSLType, new { htmlAttributes = new { @class = "xdsl-type-selection" } }).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                // xdsl type validation row
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "2");
                        cell.InnerHtml = helper.ValidationMessageFor(modelItem => model.XDSLType).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                // speed row
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(modelItem => model.SpeedName).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.Select(modelItem => model.SpeedCode, new SelectList(speedList, "Value", "Name", model.SpeedCode), RadiusR.Localization.Helpers.Common.Select, null, new { @class = "speed-selection" }).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                // speed validation row
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "2");
                        cell.InnerHtml = helper.ValidationMessageFor(modelItem => model.SpeedCode).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                // tariff row
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.LabelFor(modelItem => model.TariffName).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.InnerHtml = helper.Select(null, new SelectList(speedTariffsList, "Value", "Name", (model.TariffCode.HasValue && model.PacketCode.HasValue) ? model.TariffCode + "," + model.PacketCode : null), RadiusR.Localization.Helpers.Common.Select, null, new { @class = "tariff-selection" }).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                // tariff validation row
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "2");
                        cell.InnerHtml = helper.ValidationMessageFor(modelItem => model.TariffCode).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                // paperwork row
                {
                    TagBuilder row = new TagBuilder("tr");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "2");
                        cell.InnerHtml = helper.EditorFor(modelItem => model.IsPaperworkNeeded).ToHtmlString();
                        row.InnerHtml += cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }
                // tariff hidden row
                {
                    TagBuilder row = new TagBuilder("tr");
                    row.MergeAttribute("style", "display: none;");
                    {
                        TagBuilder cell = new TagBuilder("td");
                        cell.MergeAttribute("colspan", "2");
                        cell.InnerHtml += helper.HiddenFor(modelItem => model.PacketCode, new { @class = "packet-code-hidden" }).ToHtmlString() + helper.HiddenFor(modelItem => model.TariffCode, new { @class = "tariff-code-hidden" }).ToHtmlString();
                        row.InnerHtml = cell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += row.ToString(TagRenderMode.Normal);
                }

                wrapper.InnerHtml += table.ToString(TagRenderMode.Normal);
            }

            helper.ViewData.TemplateInfo.HtmlFieldPrefix = oldPrefix;
            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}
