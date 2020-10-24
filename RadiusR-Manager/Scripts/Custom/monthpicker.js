$(document).ready(function () {
    setupMonthpickers();
});

function setupMonthpickers(containerId) {
    var container = $("body");
    if (typeof containerId != "undefined")
        container = $(containerId);

    var setupIndividuals = function (currentWrapper) {
        var wrapper = $(currentWrapper);
        var button = wrapper.find(".date-picker-button").first();
        var dateHidden = wrapper.find("input[type=hidden]");
        var yearValue = dateHidden.val().split("-")[0];
        var monthValue = dateHidden.val().split("-")[1];
        var popup = wrapper.find(".date-picker-popup").first();
        var monthsView = popup.find(".calendar-months").first();
        var clearButton = wrapper.find(".clear-button").first();

        button.click(function () {
            popup.fadeToggle(400);
            button.toggleClass("selected");
        });
        clearButton.click(function () {
            dateHidden.val("");
            button.val(wrapper.attr("no-value-text"));
        });

        var yearDiv = monthsView.find(".calendar-year").first();
        var yearInput = monthsView.find(".calendar-year-input").first();
        var y_prev = monthsView.find(".calendar-previous-button").first();
        var y_next = monthsView.find(".calendar-next-button").first();

        y_prev.click(function () {
            yearDiv.html(parseInt(yearDiv.html()) - 1);
            yearInput.val(yearDiv.html());
            setValue();
        });
        y_next.click(function () {
            yearDiv.html(parseInt(yearDiv.html()) + 1);
            yearInput.val(yearDiv.html());
            setValue();
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
                    setValue();
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
                monthsViewMonth.removeClass("selected");
                current.addClass("selected");
                setValue();
                popup.fadeOut(200);
            });
            current.keydown(function (e) {
                if (e.keyCode == 13 || e.keyCode == 32)
                    current.click();
            });
        });

        function setValue() {
            if (monthsViewMonth.filter(".selected").length > 0) {
                dateHidden.val(yearDiv.text() + "-" + monthsViewMonth.filter(".selected").attr("value"));
                button.val(yearDiv.text() + " " + monthsViewMonth.filter(".selected").text());
            }
        }

        if (dateHidden.val() != "") {
            monthsViewMonth.filter("[value=" + monthValue + "]").addClass("selected");
            button.val(yearValue + " " + monthsViewMonth.filter(".selected").text());
        }
    };


    container.find(".month-picker-wrapper").each(function () {
        setupIndividuals($(this));
    });
}
