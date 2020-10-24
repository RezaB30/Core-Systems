function SetupAddressEditors(containerId) {
    var container = $(containerId);
    var editors = container.find('div.address-editor-container');

    editors.each(function () {
        var currentContainer = $(this);
        var updatingRows = currentContainer.find('tr.changing-list-row');
        var bbkInput = currentContainer.find('input.bbk-input');
        var bbkFetchButton = currentContainer.find('.bbk-fetch-button');
        var bbkValidationMessage = currentContainer.find('td.bbk-validation-error');
        bbkFetchButton.click(function () {
            bbkValidationMessage.text('');
            var fetchLink = bbkFetchButton.closest('tr').children('td').last().text();
            if (bbkInput.val() != '') {
                bbkFetchButton.hide();
                // load
                GetJson(addressTextFetchLink, { id: bbkInput.val() }, 'POST', function (data) {
                    if (data == null || data.ErrorOccured) {
                        // show error
                        bbkValidationMessage.text(data.ErrorMessage);
                    }
                    else {
                        var provinceInput = currentContainer.find('input[name$="ProvinceID"]');
                        var eventData = [data.Data.DistrictID, data.Data.RuralCode, data.Data.NeighbourhoodID, data.Data.StreetID, data.Data.DoorID, data.Data.ApartmentID]
                        // trigger
                        selectOption('', provinceInput.closest('div.select-list-wrapper'));
                        selectOption(data.Data.ProvinceID, provinceInput.closest('div.select-list-wrapper'), eventData);
                    }
                    setTimeout(function () { bbkFetchButton.show(); }, 3000);
                });
            }
        });
        // setup changing list
        updatingRows.each(function () {
            var currentRow = $(this);
            var currentChangingInput = currentRow.find('.select-list-wrapper.address-code').find('input[type=hidden]');
            var fetchLink = currentRow.children('td').last().text();
            var changingList = currentRow.next('tr').find('.select-list-wrapper.address-code');
            var changingInput = changingList.find('input[type=hidden]');
            var changeListItemsContainer = changingList.find('.options-container').children();
            var addressName = currentRow.find('input.address-name');
            var statusSymbol = currentRow.find('div.address-checkmark');

            currentChangingInput.change(function (e) {
                var currentChangingAddressName = currentRow.find('.select-list-wrapper.address-code').find('.list-option.selected').text();
                addressName.val('');
                changeListItemsContainer.find('.list-option:not([value=""])').remove();
                SetupSelectLists(changingList.parent());
                selectOption('', changingList);
                if (currentChangingInput.val() != '') {
                    addressName.val(currentChangingAddressName);
                    statusSymbol.removeClass('error valid invalid');
                    statusSymbol.addClass('loading');
                    GetJson(fetchLink, { id: currentChangingInput.val() }, 'POST', function (itemList) {
                        changeListItemsContainer.find('.list-option:not([value=""])').remove();
                        if (itemList == null || itemList.ErrorOccured) {
                            // show error
                            statusSymbol.removeClass('valid invalid loading');
                            statusSymbol.addClass('error');
                            return;
                        }
                        for (var i = 0; i < itemList.Data.length; i++) {
                            changeListItemsContainer.append('<div class="list-option" value="' + itemList.Data[i].Code + '">' + itemList.Data[i].Name + '</div>');
                        }
                        SetupSelectLists(changingList.parent());
                        // if it is bbk fill (has trigger queue)
                        if (e.eventData != null && e.eventData.length > 0) {
                            selectOption(e.eventData.shift(), changingList, e.eventData);
                        }
                        else {
                            // no trigger queue
                            selectOption('', changingList);
                        }
                        statusSymbol.removeClass('error invalid loading');
                        statusSymbol.addClass('valid');
                    });
                }
                else {
                    statusSymbol.removeClass('valid error loading');
                    statusSymbol.addClass('invalid');
                }
            });
        });

        // setup result change
        var addressResultRow = currentContainer.find('tr.address-result-change');
        var addressTextFetchLink = addressResultRow.children('td').last().text();
        var addressCode = currentContainer.find('.address-code-display');
        var addressText = currentContainer.find('.address-text-display');
        var addressNoText = currentContainer.find('.address-no-display');
        var addressTextInput = currentContainer.find('input.address-text-input');
        addressResultRow.find('.select-list-wrapper.address-code').find('input[type=hidden]').change(function () {
            var codeInput = $(this);
            var statusSymbol = addressResultRow.find('div.address-checkmark');
            var addressName = addressResultRow.find('input.address-name');
            var addressNoInput = currentContainer.find('input.address-no-input');
            var currentChangingAddressName = addressResultRow.find('.select-list-wrapper.address-code').find('.list-option.selected').text();
            addressName.val('');
            addressCode.text(codeInput.val());
            addressText.text('');
            addressTextInput.val('');
            addressNoInput.val('');
            addressNoText.text('');
            addressNoInput.trigger('change');
            if (codeInput.val() != '') {
                addressName.val(currentChangingAddressName);
                statusSymbol.removeClass('error valid invalid');
                statusSymbol.addClass('loading');
                GetJson(addressTextFetchLink, { id: codeInput.val() }, 'POST', function (data) {
                    if (data == null || data.ErrorOccured) {
                        // show error
                        statusSymbol.removeClass('valid invalid loading');
                        statusSymbol.addClass('error');
                    }
                    else {
                        addressText.text(data.Data.AddressText);
                        addressTextInput.val(data.Data.AddressText);
                        addressNoInput.val(data.Data.AddressNo);
                        addressNoText.text(data.Data.AddressNo);
                        statusSymbol.removeClass('error invalid loading');
                        statusSymbol.addClass('valid');
                        addressNoInput.trigger('change');
                    }
                });
            }
            else {
                statusSymbol.removeClass('valid error loading');
                statusSymbol.addClass('invalid');
            }
        });

        // setup current status symbols
        currentContainer.find('tr.changing-list-row,tr.address-result-change').each(function () {
            var currentRow = $(this);
            var statusSymbol = currentRow.find('div.address-checkmark');
            var currentValue = currentRow.find('.select-list-wrapper.address-code').find('input[type=hidden]').val();
            if (currentValue == '') {
                statusSymbol.removeClass('valid error loading');
                statusSymbol.addClass('invalid');
            }
            else {
                statusSymbol.removeClass('invalid loading error');
                statusSymbol.addClass('valid');
            }
        });
    });
}