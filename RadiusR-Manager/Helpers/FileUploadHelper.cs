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

        public static MvcHtmlString MultiFileUpload(this HtmlHelper helper, string fieldName, string accept = null)
        {
            //wrapper
            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("multi-file-upload-wrapper");
            // sample
            {
                TagBuilder sampleContainer = new TagBuilder("div");
                sampleContainer.MergeAttribute("style", "display: none;");
                sampleContainer.AddCssClass("sample-container");
                {
                    TagBuilder instanceRow = new TagBuilder("div");
                    instanceRow.AddCssClass("instance-row");
                    instanceRow.InnerHtml = helper.FileUpload(fieldName, accept).ToHtmlString();
                    TagBuilder removeButton = new TagBuilder("input");
                    removeButton.MergeAttributes(new Dictionary<string, string>()
                    {
                        { "type", "button" },
                        { "value", RezaB.Web.Helpers.Localization.Common.Remove }
                    });
                    removeButton.AddCssClass("link-button iconed-button delete-button");
                    instanceRow.InnerHtml += removeButton.ToString(TagRenderMode.SelfClosing);
                    sampleContainer.InnerHtml = instanceRow.ToString(TagRenderMode.Normal);
                }

                wrapper.InnerHtml += sampleContainer.ToString(TagRenderMode.Normal);
            }
            // rows
            {
                TagBuilder rowsContainer = new TagBuilder("div");
                rowsContainer.AddCssClass("rows-container");
                wrapper.InnerHtml += rowsContainer.ToString(TagRenderMode.Normal);
            }
            // add instance
            {
                TagBuilder addInstanceRow = new TagBuilder("div");
                addInstanceRow.AddCssClass("add-instance-row");
                TagBuilder addInstanceButton = new TagBuilder("input");
                addInstanceButton.MergeAttributes(new Dictionary<string, string>()
                {
                    { "type", "button" },
                    { "value", RezaB.Web.Helpers.Localization.Common.AddInstance }
                });
                addInstanceButton.AddCssClass("link-button iconed-button add-instance-button");
                addInstanceRow.InnerHtml = addInstanceButton.ToString(TagRenderMode.SelfClosing);
                wrapper.InnerHtml += addInstanceRow.ToString(TagRenderMode.Normal);
            }

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}