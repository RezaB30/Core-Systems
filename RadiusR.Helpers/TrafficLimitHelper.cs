using RezaB.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace RadiusR.Helpers
{
    public static class TrafficLimitHelper
    {
        public static MvcHtmlString TrafficLimit(this HtmlHelper helper, string fieldname, string value = null)
        {
            MixedResults mixedValue = null;
            long parsed;
            if (!long.TryParse(value, out parsed))
            {
                parsed = -1;
            }

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("traffic-limit-wrapper");

            TagBuilder hidden = new TagBuilder("input");
            hidden.MergeAttribute("name", fieldname);
            hidden.MergeAttribute("type", "hidden");
            hidden.AddCssClass("traffic-limit-hidden");
            wrapper.InnerHtml += hidden.ToString(TagRenderMode.SelfClosing);

            TagBuilder textbox = new TagBuilder("input");
            textbox.GenerateId(fieldname);
            textbox.MergeAttribute("autocomplete", "off");
            textbox.MergeAttribute("type", "text");
            textbox.AddCssClass("traffic-limit-text");
            if (parsed > 0)
            {
                mixedValue = GetQuotient(parsed);
                textbox.MergeAttribute("value", mixedValue.fieldValue.ToString());
            }
            wrapper.InnerHtml += textbox.ToString(TagRenderMode.SelfClosing);

            {
                var rateItems = new List<Rates>();
                rateItems.Add(new Rates { Name = "B", Value = "0" });
                rateItems.Add(new Rates { Name = "KB", Value = "1" });
                rateItems.Add(new Rates { Name = "MB", Value = "2" });
                rateItems.Add(new Rates { Name = "GB", Value = "3" });
                rateItems.Add(new Rates { Name = "TB", Value = "4" });

                string selectedValue = (mixedValue == null) ? "0" : mixedValue._suffix.ToString();

                wrapper.InnerHtml += helper.Select("", new SelectList(rateItems, "Value", "Name", selectedValue));
            }

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString TrafficLimit<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, string value = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);

            return TrafficLimit(helper, fullName, value);
        }

        private class Rates
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }

        private static MixedResults GetQuotient(long raw)
        {
            long remainder = 0;
            var quotient = raw;
            var stage = 0;
            do
            {
                quotient = raw / 1024;
                remainder = raw % 1024;
                if (remainder == 0)
                {
                    raw = quotient;
                    stage++;
                }
            } while (remainder == 0 && stage < 4);
            return new MixedResults()
            {
                fieldValue = raw,
                _suffix = stage
            };
        }

        private class MixedResults
        {
            public long fieldValue { get; set; }

            public int _suffix { get; set; }

            public string Suffix
            {
                get
                {
                    switch (_suffix)
                    {
                        case 1:
                            return "K";
                        case 2:
                            return "M";
                        case 3:
                            return "G";
                        case 4:
                            return "T";
                        default:
                            return "B";
                    }
                }
            }
        }
    }
}