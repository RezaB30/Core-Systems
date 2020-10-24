using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Helpers
{
    public static class FileUploadHelper
    {
        public static MvcHtmlString FileUpload(this HtmlHelper helper, string fieldName, string accept = null)
        {
            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("file-upload-wrapper");

            TagBuilder hiddenFile = new TagBuilder("input");
            hiddenFile.AddCssClass("hidden-file-upload");
            hiddenFile.MergeAttribute("type", "file");
            hiddenFile.GenerateId(fieldName);
            hiddenFile.MergeAttribute("name", fieldName);
            if (!string.IsNullOrEmpty(accept))
            {
                hiddenFile.MergeAttribute("accept", accept);
            }

            TagBuilder fileName = new TagBuilder("input");
            fileName.AddCssClass("file-upload-text");
            fileName.MergeAttribute("type", "text");
            fileName.MergeAttribute("readonly", "readonly");

            TagBuilder fileButton = new TagBuilder("input");
            fileButton.AddCssClass("upload-file-browse");
            fileButton.MergeAttribute("type", "button");
            fileButton.AddCssClass("link-button iconed-button browse-button");
            fileButton.MergeAttribute("value", RadiusR.Localization.Helpers.Common.Browse);

            wrapper.InnerHtml = hiddenFile.ToString(TagRenderMode.SelfClosing)
                + fileName.ToString(TagRenderMode.SelfClosing) + new MvcHtmlString("&nbsp;")
                + fileButton.ToString(TagRenderMode.SelfClosing);

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}