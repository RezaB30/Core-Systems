var calendarDataLink;

function setCalendarLink(_calendarLink) {
    calendarDataLink = _calendarLink;
}

$(document).ready(function () {
    setupDatepickers();
});

function setupDatepickers(containerId) {
    var container = $("body");
    if (typeof containerId != "undefined")
        container = $(containerId);

    var setupIndividuals = function (currentWrapper) {
        var wrapper = $(currentWrapper);
        var button = wrapper.find(".date-picker-button").first();
        var hidden = wrapper.find("input[type=hidden]").first();
        var popup = wrapper.find(".date-picker-popup").first();
        var monthsView = popup.find(".calendar-months").first();
        var daysView = popup.find(".calendar-selected-month").first();
        var cover = popup.find(".calendar-loading-cover").first();
        var clearButton = wrapper.find(".clear-button").first();

        var selectedYearNo = null;
        var selectedMonthNo = null;
        var selectedDayNo = null;
        if (button.attr("value") != "") {
            selectedDayNo = (button.attr("value") + "");
            selectedDayNo = selectedDayNo.split(".");
            if (selectedDayNo.length == 3) {
                selectedYearNo = selectedDayNo[2];
                selectedMonthNo = selectedDayNo[1];
                selectedDayNo = selectedDayNo[0];
            }
            else
                selectedDayNo = null;
        }

        button.click(function () {
            if (selectedDayNo != null && popup.is(":hidden")) {
                monthsView.hide();
                daysView.show();
                cover.fadeIn(200);
                LoadCalendar(function () {
                    setupDaysView();
                    monthsView.hide();
                    daysView.fadeIn(200);
                    cover.fadeOut(200);
                },
                selectedYearNo,
                selectedMonthNo,
                selectedDayNo);
            }
            else {
                monthsView.show();
                daysView.hide();
            }
            popup.fadeToggle(400);
            button.toggleClass("selected");
        });
        clearButton.click(function () {
            hidden.val("");
            button.val(wrapper.attr("no-value-text"));
            selectedDayNo = selectedMonthNo = selectedYearNo = null;
        });

        var yearDiv = monthsView.find(".calendar-year").first();
        var yearInput = monthsView.find(".calendar-year-input").first();
        var y_prev = monthsView.find(".calendar-previous-button").first();
        var y_next = monthsView.find(".calendar-next-button").first();

        y_prev.click(function () {
            yearDiv.html(parseInt(yearDiv.html()) - 1);
            yearInput.val(yearDiv.html());
        });
        y_next.click(function () {
            yearDiv.html(parseInt(yearDiv.html()) + 1);
            yearInput.val(yearDiv.html());
        });

        yearDiv.click(function () {
            yearInput.val(yearDiv.html());
            yearInput.fadeIn(200);
            yearDiv.hide();
            yearInput.select();
        });

        yearDiv.keydown(function (e) {
            if (e.keyCode == 13 || e.keyCode == 32) {
                yearDiv.click();
                e.preventDefault();
            }
        });

        yearInput.keydown(function (e) {
            switch (e.which) {
                case 13:
                    e.preventDefault();
                    yearDiv.text(yearInput.val());
                    yearInput.hide();
                    yearDiv.fadeIn(200);
                    break;
                case 27:
                    e.preventDefault();
                    yearDiv.fadeIn(200);
                    yearInput.hide();
                    break;
                default:
                    break;
            }
        });

        var monthsViewMonth = monthsView.find("td");
        monthsViewMonth.each(function () {
            var current = $(this);
            current.click(function () {
                LoadCalendar(function () {
                    monthsView.hide();
                    daysView.fadeIn(200, function () { setupDaysView(); });
                    cover.fadeOut(200);
                },
                yearDiv.html(),
                current.attr("value"),
                setDayNoForHighlight(yearDiv.html(), current.attr("value")));
            });
            current.keydown(function (e) {
                if (e.keyCode == 13 || e.keyCode == 32)
                    current.click();
            });
        });

        var setupDaysView = function () {
            var m_prev = daysView.find(".calendar-previous-button").first();
            var m_next = daysView.find(".calendar-next-button").first();
            var monthDiv = daysView.find(".calendar-month").first();
            var monthNo = parseInt(monthDiv.attr("value"));
            var m_yearDiv = daysView.find(".calendar-year").first();
            var yearNo = parseInt(m_yearDiv.html());
            var allDays = daysView.find("tr").slice(2).find("td:visible");

            m_prev.click(function () {
                monthNo--;
                if (monthNo < 1) {
                    monthNo = 12;
                    yearNo--;
                }
                LoadCalendar(function () {
                    setupDaysView();
                },
                yearNo,
                monthNo,
                setDayNoForHighlight(yearNo, monthNo));
            });
            m_next.click(function () {
                monthNo++;
                if (monthNo > 12) {
                    monthNo = 1;
                    yearNo++;
                }
                LoadCalendar(function () {
                    setupDaysView();
                },
                yearNo,
                monthNo,
                setDayNoForHighlight(yearNo, monthNo));
            });
            monthDiv.click(function () {
                yearDiv.html(m_yearDiv.html());
                yearInput.val(m_yearDiv.html());
                daysView.hide();
                monthsView.fadeIn(200);
            });
            monthDiv.keydown(function (e) {
                if (e.keyCode == 13 || e.keyCode == 32)
                    monthDiv.click();
            });

            allDays.click(function () {
                var selectedDay = $(this);
                allDays.removeClass("selected");
                selectedDay.addClass("selected");
                button.attr("value", selectedDay.html() + "." + monthNo + "." + yearNo);
                hidden.val(button.attr("value"));
                popup.fadeOut(200);
                wrapper.find("*").off();
                setupIndividuals(wrapper);
            });
            allDays.keydown(function (e) {
                if (e.keyCode == 13 || e.keyCode == 32)
                    $(this).click();
            });
        };

        var LoadCalendar = function (callback, year, month, day) {
            var data = {};
            if (!(day === undefined) && day != null) {
                data = {
                    year: year,
                    month: month,
                    day: day
                }
            }
            else {
                data = {
                    year: year,
                    month: month
                }
            }

            cover.fadeIn(200);
            $.ajax({
                url: calendarDataLink,
                method: "GET",
                data: data,
                success: function (response) {
                    daysView.html(response);
                    callback();
                    cover.fadeOut(200);
                },
                error: function (jqXHR, exception) {
                    HandleAjaxError(jqXHR, exception);
                    cover.fadeOut(200);
                }
            });
        };

        var setDayNoForHighlight = function (year, month) {
            year += "";
            month += "";
            if (year == selectedYearNo && month == selectedMonthNo)
                return selectedDayNo;
            return null;
        };
    };


    container.find(".date-picker-wrapper").each(function () {
        setupIndividuals($(this));
    });
}
