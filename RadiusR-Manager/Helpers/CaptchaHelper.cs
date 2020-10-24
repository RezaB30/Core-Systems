using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Helpers
{
    public static class CaptchaHelper
    {
        /// <summary>
        /// Generates captcha.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="fieldName">Name of captcha key field.</param>
        /// <param name="showValidationMessage">If validation message must be shown.</param>
        /// <returns></returns>
        public static MvcHtmlString Captcha(this HtmlHelper helper, string fieldName, bool? showValidationMessage = false)
        {
            UrlHelper url = new UrlHelper(helper.ViewContext.RequestContext);

            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("captcha");

            TagBuilder imageWrapper = new TagBuilder("div");
            imageWrapper.AddCssClass("captcha-image-wrapper");

            TagBuilder image = new TagBuilder("img");
            image.MergeAttribute("src", url.Action("Index", "Captcha"));
            image.GenerateId("captcha");

            TagBuilder reloadButton = new TagBuilder("input");
            reloadButton.MergeAttribute("type", "button");
            reloadButton.MergeAttribute("onclick", "javascript: reload_captcha();");

            imageWrapper.InnerHtml = image.ToString(TagRenderMode.SelfClosing)
                + reloadButton.ToString(TagRenderMode.SelfClosing);

            TagBuilder captchaText = new TagBuilder("input");
            captchaText.MergeAttribute("type", "text");
            captchaText.GenerateId(fieldName);
            captchaText.MergeAttribute("name", fieldName);

            TagBuilder breakTag = new TagBuilder("br");

            TagBuilder validationMessage = new TagBuilder("span");
            validationMessage.AddCssClass("text-danger");
            validationMessage.SetInnerText(RadiusR.Localization.Helpers.Common.CaptchaError);
            if (!showValidationMessage.HasValue || !showValidationMessage.Value)
            {
                validationMessage.MergeAttribute("style", "visibility: hidden;");
            }

            wrapper.InnerHtml = imageWrapper.ToString(TagRenderMode.Normal) +
                captchaText.ToString(TagRenderMode.SelfClosing) + breakTag.ToString(TagRenderMode.SelfClosing) +
                validationMessage.ToString(TagRenderMode.Normal);

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }
    }
}