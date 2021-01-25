///<reference path="Scripts/jquery-{version}.js" />
///<reference path="Scripts/jquery-{version}.intellisense.js" />
///<reference path="Scripts/async-view.js" />
///<reference path="Scripts/dialogbox.js" />

function SetupSelectLists(containerId) {
    ///<summary>Enables interactions with custom select list.</summary>
    /// <param name="containerId" type="String">Selection list container id</param>
    var slidingTime = 200;
    var workArea = $(containerId);
    workArea.find(".select-list-wrapper").each(function () {
        var currentWrapper = $(this);
        var textbox = currentWrapper.find("input[type='text']").first();
        var hiddenField = currentWrapper.find("input[type='hidden']").first();
        var dropArrow = currentWrapper.find(".drop-down-arrow");
        var optionsContainer = currentWrapper.find(".options-container");
        var allOptions = currentWrapper.find(".list-option");

        textbox.off();
        currentWrapper.find().off();

        selectOption(hiddenField.val(), currentWrapper);

        dropArrow.click(function () {
            textbox.select();
            textbox.trigger('click');
        });

        textbox.focusin(function () {
            textbox.select();
        });

        textbox.click(function (event) {
            optionsContainer.stop(true, false);
            optionsContainer.slideToggle(slidingTime, function () { optionsContainer.height('') });
        });

        textbox.keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                setValue();
                if (optionsContainer.is(':hidden'))
                    textbox.select();
                textbox.click();
            }
            else if (event.keyCode == 38) {
                event.preventDefault();
                var selectedOption = optionsContainer.find(".selected:not(.hidden)");
                if (selectedOption.length < 1) {
                    selectedOption = optionsContainer.find(".list-option:not(.hidden)").last();
                    selectedOption.addClass('selected');
                }
                else {
                    var nextOption = selectedOption.prevAll(".list-option:not(.hidden)").first();
                    if (nextOption.length == 1) {
                        nextOption.addClass("selected");
                        selectedOption.removeClass("selected");
                        optionsContainer.scrollTop(nextOption.offset().top -
                            nextOption.parent().offset().top +
                            nextOption.parent().scrollTop());
                    }
                }

                if (optionsContainer.is(':not(:visible)')) {
                    setValue();
                }
            }
            else if (event.keyCode == 40) {
                event.preventDefault();
                var selectedOption = optionsContainer.find(".selected:not(.hidden)");
                if (selectedOption.length < 1) {
                    selectedOption = optionsContainer.find(".list-option:not(.hidden)").first();
                    selectedOption.addClass('selected');
                }
                else {
                    var nextOption = selectedOption.nextAll(".list-option:not(.hidden)").first();
                    if (nextOption.length == 1) {
                        nextOption.addClass("selected");
                        selectedOption.removeClass("selected");
                        optionsContainer.scrollTop(nextOption.offset().top -
                            nextOption.parent().offset().top +
                            nextOption.parent().scrollTop());
                    }
                }
                if (optionsContainer.is(':not(:visible)')) {
                    setValue();
                }
            }
            else if (event.keyCode == 27) {
                textbox.trigger('focusout');
            }
        });

        // search
        textbox.bind("input", function () {
            allOptions.each(function () {
                if ($(this).text().toLocaleLowerCase().includes(textbox.val().toLocaleLowerCase()) && $(this).attr('value') != "") {
                    $(this).removeClass('hidden');
                }
                else {
                    $(this).addClass('hidden');
                }
                allOptions.removeClass('selected');
                allOptions.filter(':not(.hidden)').first().addClass('selected');
            });
        });

        allOptions.click(function (event) {
            allOptions.removeClass("selected");
            $(this).addClass("selected");
            setValue();
        });

        setupListButton(currentWrapper);
        setupSearchButton(currentWrapper);

        textbox.focusout(function () {
            //resetSelection();
            optionsContainer.delay(200).slideUp(slidingTime, resetSelection);
        });


        // utility functions
        function setValue() {
            var currentOption = currentWrapper.find(".selected").first();

            var hasChanged = false;
            if (currentOption.attr("value") != hiddenField.val())
                hasChanged = true;

            textbox.val(currentOption.text());
            hiddenField.val(currentOption.attr("value"));

            if (hasChanged) {
                hiddenField.trigger("change");
                textbox.trigger("change");
            }
        }

        function resetSelection() {
            selectOption(hiddenField.val(), currentWrapper);
            allOptions.removeClass('hidden');
        }

        function selectOption(optionValue, wrapper) {
            //var allOptions = currentWrapper.find(".list-option");
            var targetOption = currentWrapper.find(".list-option[value='" + optionValue + "']");
            allOptions.removeClass("selected");
            targetOption.addClass("selected");
            setValue();
        }

        function selectCurrentOption() {
            selectOption(allOptions.filter('.selected').first(), currentWrapper);
        }

        function setupSearchButton(currentWrapper) {
            var searchButton = $(currentWrapper).find('input.add-instance-button');
            if (searchButton.length > 0) {
                currentWrapper.css({ 'margin-right': '5em' });
                createSearchForm();
                var form = currentWrapper.next('div.select-list-search-form');
                searchButton.click(function (e) {
                    e.stopPropagation();
                    form.addClass('open');
                    form.find('div.search-box input[type="text"]').focus();
                    form.find('div.search-box input[type="text"]').select();
                });
            }

            function createSearchForm() {
                currentWrapper.after('<div class="select-list-search-form"></div>');
                var formWrapper = currentWrapper.next('div.select-list-search-form');
                formWrapper.append('<div class="form-inner"></div>');
                var form = formWrapper.find('div.form-inner');
                form.append('<div class="search-box"><input type="text"/></div>');
                var searchBox = form.find('div.search-box input');
                searchBox.after('<input type="button" class="link-button iconed-button remove-button"/>');
                var closeButton = searchBox.next('input');
                var allOptions = currentWrapper.find(".list-option");
                allOptions.each(function () {
                    var currentItem = $(this);
                    if (currentItem.attr('value') == '')
                        return;
                    form.append('<div class="list-item" value="' + currentItem.attr("value") + '">' + currentItem.text().toUpperCase() + '<div>');
                });

                closeButton.click(function () {
                    formWrapper.removeClass('open');
                });

                var newOptions = form.find('div.list-item');
                newOptions.click(function () {
                    var currentItem = $(this);
                    formWrapper.removeClass('open');
                    selectOption(currentItem.attr('value'), currentWrapper);
                });

                searchBox.bind('input', function () {
                    var value = searchBox.val();
                    if (value == '') {
                        newOptions.show();
                    }
                    else {
                        newOptions.hide();
                        newOptions.filter(':contains("' + value.toUpperCase() + '")').each(function () {
                            $(this).show();
                        });
                    }
                });
            }
        }
    });
}

function setValue(wrapper, eventData) {
    var currentWrapper = $(wrapper);
    var textbox = currentWrapper.find("input[type='text']").first();
    var hiddenField = currentWrapper.find("input[type='hidden']").first();
    var currentOption = currentWrapper.find(".selected").first();

    var hasChanged = false;
    if (currentOption.attr("value") != hiddenField.val())
        hasChanged = true;

    textbox.val(currentOption.text());
    hiddenField.val(currentOption.attr("value"));

    if (hasChanged) {
        hiddenField.trigger({ type: "change", eventData: eventData });
        textbox.trigger("change");
    }
}

function ClearSelectListItems(wrapper) {
    $(wrapper).find('div.options-container').children().first().find('div.list-option:not([value=""])').remove();
    selectOption('', wrapper);
}

function selectOption(optionValue, wrapper, eventData) {
    //if (optionValue == "")
    //    return;
    var allOptions = $(wrapper).find(".list-option");
    var targetOption = $(wrapper).find(".list-option[value='" + optionValue + "']");
    if (targetOption.length == 1) {
        allOptions.removeClass("selected");
        targetOption.addClass("selected");
        setValue(wrapper, eventData);
    }
}

function setupListButton(containerObject) {
    var currentWrapper = $(containerObject);
    if (currentWrapper.parents(".multiselect-sample").length > 0) {
        return;
    }
    var dataContainer = currentWrapper.find("input[type=hidden]").first();

    var button = currentWrapper.parent().find(".list-button").first();
    var listLink = button.attr("href");
    button.attr("href", "javascript:void(0);");

    var newId = currentWrapper.parent().find("input[type=hidden]").attr("name") + "_select-list-container";

    button.click(function () {
        DialogBox(newId, function () {
            var viewLoader = new asyncView(listLink, "#" + newId);
            viewLoader.addTableButtonCallback(function () {
                $("#" + newId).find(".select-button").each(function () {
                    var selectButton = $(this);
                    selectButton.click(function () {
                        selectOption(selectButton.attr("value"), currentWrapper);
                        CloseDialog(newId);
                    });
                });
            });
            viewLoader.LoadView();
        });
    });
}

function FillSelectListFromUrl(wrapper, loadUrl, parameters, callback, selectedValue) {
    var currentWrapper = $(wrapper);
    var optionsContainer = currentWrapper.find('div.options-container').children().first();
    $.ajax(loadUrl, {
        data: parameters,
        method: 'POST',
        complete: function (data, status) {
            ClearSelectListItems(currentWrapper);
            var items = null;
            if (status == "success") {
                items = data.responseJSON.Items;
                items.forEach(function (currentItem) {
                    optionsContainer.append('<div class="list-option" value="' + currentItem.Value + '">' + currentItem.Name + '<div>');
                });
                if (data.responseJSON.selectedValue != null) {
                    selectOption(data.responseJSON.selectedValue, currentWrapper);
                }
                if (selectedValue != null) {
                    selectOption(selectedValue, currentWrapper);
                }
            }
            SetupSelectLists(currentWrapper.parent());
            callback(status, items);
        }
    })
}