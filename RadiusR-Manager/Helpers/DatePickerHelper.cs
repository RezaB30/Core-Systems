using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace RadiusR_Manager.Helpers
{
    public static class DatePickerHelper
    {
        /// <summary>
        /// Creates a date picker.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="propName">Name of field.</param>
        /// <param name="selectedDate">Selected date on calendar.</param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString DatePicker(this HtmlHelper helper, string propName, DateTime? selectedDate = null, IDictionary<string, string> htmlAttributes = null)
        {
            var hasValue = selectedDate != null && selectedDate.Value.Ticks > 0;

            var wrapper = new TagBuilder("div");
            wrapper.AddCssClass("date-picker-wrapper");
            wrapper.MergeAttribute("tabindex", "0");
            wrapper.MergeAttribute("no-value-text", RadiusR.Localization.Helpers.Common.Select);

            var hidden = new TagBuilder("input");
            hidden.MergeAttribute("type", "hidden");
            hidden.MergeAttribute("name", propName);
            hidden.GenerateId(propName);

            var button = new TagBuilder("input");
            button.MergeAttribute("type", "button");
            button.AddCssClass("link-button date-picker-button");

            var popupContainer = new TagBuilder("div");
            popupContainer.MergeAttribute("style", "display: none;");
            popupContainer.AddCssClass("date-picker-popup");

            if (hasValue)
            {
                popupContainer.InnerHtml = CreateLocalizedCalendar(selectedDate).ToString(TagRenderMode.Normal);
            }
            else
            {
                popupContainer.InnerHtml = CreateLocalizedCalendar().ToString(TagRenderMode.Normal);
            }


            if (hasValue)
            {
                var dateString = selectedDate.Value.ToString("dd.MM.yyyy", new CultureInfo("tr-tr"));
                button.MergeAttribute("value", dateString);
                hidden.MergeAttribute("value", dateString);
            }
            else
            {
                button.MergeAttribute("value", RadiusR.Localization.Helpers.Common.Select);
            }

            if (htmlAttributes != null)
            {
                foreach (var htmlAttribute in htmlAttributes)
                {
                    button.Attributes.Add(htmlAttribute);
                }
            }

            var clearButton = new TagBuilder("input");
            clearButton.MergeAttribute("type", "button");
            clearButton.AddCssClass("link-button");
            clearButton.AddCssClass("iconed-button");
            clearButton.AddCssClass("clear-button");

            wrapper.InnerHtml = hidden.ToString(TagRenderMode.SelfClosing) +
                button.ToString(TagRenderMode.SelfClosing) +
                popupContainer.ToString(TagRenderMode.Normal) +
                clearButton.ToString(TagRenderMode.SelfClosing);

            return new MvcHtmlString(wrapper.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Creates a date picker.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="helper"></param>
        /// <param name="expression"></param>
        /// <param name="selectedDate">Selected date on calendar.</param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        public static MvcHtmlString DatePicker<TModel, TResult>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TResult>> expression, DateTime? selectedDate = null, IDictionary<string, string> htmlAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fullName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);

            return DatePicker(helper, fullName, selectedDate, htmlAttributes);
        }

        private static TagBuilder CreateLocalizedCalendar(DateTime? selectedDate = null)
        {
            var datetimeInfo = DateTimeFormatInfo.GetInstance(Thread.CurrentThread.CurrentCulture);

            var calendar = new TagBuilder("div");
            calendar.MergeAttribute("style", "position: relative;");

            var monthsLayer = new TagBuilder("div");
            monthsLayer.MergeAttribute("tabindex", "0");
            monthsLayer.AddCssClass("calendar-months");
            {
                var table = new TagBuilder("table");
                {
                    var tableHead = new TagBuilder("tr");
                    var tableHeadCell = new TagBuilder("th");
                    tableHeadCell.MergeAttribute("colspan", "100%");
                    var prevButton = new TagBuilder("input");
                    prevButton.MergeAttribute("type", "button");
                    prevButton.MergeAttribute("value", "<");
                    prevButton.AddCssClass("calendar-previous-button");

                    var year = new TagBuilder("div");
                    year.AddCssClass("calendar-year");
                    year.MergeAttribute("tabindex", "0");
                    if (selectedDate.HasValue)
                    {
                        year.InnerHtml = selectedDate.Value.Year.ToString();
                    }
                    else
                    {
                        year.InnerHtml = DateTime.Now.Year.ToString();
                    }

                    var yearInput = new TagBuilder("input");
                    yearInput.AddCssClass("calendar-year-input");
                    yearInput.MergeAttribute("maxlength", "4");
                    yearInput.MergeAttribute("value", year.InnerHtml);

                    var nextButton = new TagBuilder("input");
                    nextButton.MergeAttribute("type", "button");
                    nextButton.MergeAttribute("value", ">");
                    nextButton.AddCssClass("calendar-next-button");

                    tableHeadCell.InnerHtml = prevButton.ToString(TagRenderMode.SelfClosing) +
                        "&nbsp;" +
                        year.ToString(TagRenderMode.Normal) + yearInput.ToString(TagRenderMode.SelfClosing) +
                        "&nbsp;" +
                        nextButton.ToString(TagRenderMode.SelfClosing);
                    tableHead.InnerHtml = tableHeadCell.ToString(TagRenderMode.Normal);
                    table.InnerHtml += tableHead.ToString(TagRenderMode.Normal);
                }
                for (int i = 1; i < 5; i++)
                {
                    var tableRow = new TagBuilder("tr");
                    for (int j = 1; j < 4; j++)
                    {
                        var tableCell = new TagBuilder("td");
                        tableCell.MergeAttribute("tabindex", "0");
                        var monthNo = ((i - 1) * 3) + j;
                        tableCell.MergeAttribute("value", monthNo.ToString());
                        tableCell.InnerHtml = datetimeInfo.GetMonthName(monthNo);
                        tableRow.InnerHtml += tableCell.ToString(TagRenderMode.Normal);
                    }
                    table.InnerHtml += tableRow.ToString(TagRenderMode.Normal);
                }
                monthsLayer.InnerHtml += table.ToString(TagRenderMode.Normal);
            }
            calendar.InnerHtml += monthsLayer.ToString(TagRenderMode.Normal);

            var selectedMonthLayer = new TagBuilder("div");
            selectedMonthLayer.AddCssClass("calendar-selected-month");
            calendar.InnerHtml += selectedMonthLayer.ToString(TagRenderMode.Normal);

            var loadingCover = new TagBuilder("div");
            loadingCover.AddCssClass("calendar-loading-cover");
            loadingCover.MergeAttribute("style", "display: none;");
            calendar.InnerHtml += loadingCover.ToString(TagRenderMode.Normal);

            return calendar;
        }
    }
}