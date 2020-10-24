function SetupCityListUpdater(provinceContainerId, cityListContainerId, loadingUrl, initialValue) {
    var provinceListInput = $(provinceContainerId).find('.select-list-wrapper').find('input[type=hidden]');
    var cityList = $(cityListContainerId).find('.select-list-wrapper');
    var cityListInput = cityList.find('input[type=hidden]');
    var cityListOptionsContainer = cityList.find("div.options-container").first();

    function FillCityList(value) {
        // clear the list
        selectOption("", cityList);
        var currentContainer = cityListOptionsContainer.children().first();
        currentContainer.children(':not([value=""])').remove();

        if (provinceListInput.val()) {
            // retrieve cities
            $.ajax(loadingUrl, {
                context: cityList,
                data: { id: provinceListInput.val()},
                method: "POST",
                complete: function (data, status) {
                    if (status == "success") {
                        var retrievedCities = data.responseJSON;
                        var addedContent = "";

                        if (retrievedCities != null) {
                            for (var i = 0; i < retrievedCities.length; i++) {
                                addedContent = addedContent.concat("<div class='list-option' value='" + retrievedCities[i].ID + "'>" + retrievedCities[i].Name + "</div>");
                            }
                        }
                        
                        currentContainer.html(currentContainer.html() + addedContent);
                        if (value == null)
                            value = "";
                        currentContainer.children('[value="' + value + '"]').addClass('selected');
                        cityListInput.val(value);
                        $(cityList.parent()).find('*').off();
                        SetupSelectLists(cityList.parent());
                        cityListInput.trigger('change');
                    }
                }
            });
        }
    }

    provinceListInput.change(function () {
        FillCityList('');
    });

    FillCityList(initialValue);
}