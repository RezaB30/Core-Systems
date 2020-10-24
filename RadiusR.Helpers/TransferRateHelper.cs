using RezaB.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR.Helpers
{
    public static class TransferRateHelper
    {
        public static MvcHtmlString TransferRate(this HtmlHelper helper, string fieldname, int? value = null, string rateSuffix = null)
        {
            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("transfer-rate-wrapper");

            TagBuilder hidden = new TagBuilder("input");
            if (!string.IsNullOrEmpty(fieldname))
                hidden.MergeAttribute("name", fieldname);
            hidden.MergeAttribute("type", "hidden");
            hidden.AddCssClass("transfer-rate-hidden");
            wrapper.InnerHtml += hidden.ToString(TagRenderMode.SelfClosing);

            TagBuilder textbox = new TagBuilder("input");
            textbox.GenerateId(fieldname);
            textbox.MergeAttribute("autocomplete", "off");
            textbox.MergeAttribute("type", "text");
            textbox.AddCssClass("transfer-rate-text");
            if (value.HasValue)
                textbox.MergeAttribute("value", value.ToString());
            wrapper.InnerHtml += textbox.ToString(TagRenderMode.SelfClosing);

            {
                var rateItems = new List<Rates>();
                rateItems.Add(new Rates { Name = "Kbps", Value = "k" });
                rateItems.Add(new Rates { Name = "Mbps", Value = "M" });

                var selectedValue = rateSuffix;

                wrapper.InnerHtml += helper.Select("", new SelectList(rateItems, "Value", "Name", selectedValue), "bps");
            }


            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }

        private class Rates
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }
    }
}