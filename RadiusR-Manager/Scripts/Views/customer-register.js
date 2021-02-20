//TCK validation
function CustomerViewSetupTCKValidation(tckValidationUrl) {
    $('#validate-id-button').click(function () {
        var resultsLoader = new AjaxLoader('#validate-id-results', false);
        resultsLoader.Load(tckValidationUrl, $('#validate-id-button').closest('form').serialize(), 'POST');
    });
}

//customer view
function CustomerViewSetupCustomerTypeToggle(individualCustomerTypeCode) {
    $('#customer-type-container').find('div.select-list-wrapper').find('input[type="hidden"]').change(function () {
        var customerTypeInput = $(this);
        if (customerTypeInput.val() == "") {
            $('#individual-info').hide();
            $('#corporate-info').hide();
        }
        else if (customerTypeInput.val() == individualCustomerTypeCode) {
            $('#individual-info').show();
            $('#corporate-info').hide();
        }
        else {
            $('#individual-info').hide();
            $('#corporate-info').show();
        }
    });
    $('#customer-type-container').find('div.select-list-wrapper').find('input[type="hidden"]').trigger('change');
}

// addresses hide/show
function CustomerViewSetupAddressToggles() {
    $('#billing-address-same-as-setup').find('input[type=checkbox]').change(function () {
        var checkbox = $(this);
        if (checkbox.is(':checked')) {
            $('#billing-address-editor-container').hide();
        }
        else {
            $('#billing-address-editor-container').show();
        }
    });
    $('#residency-address-same-as-setup').find('input[type=checkbox]').change(function () {
        var checkbox = $(this);
        if (checkbox.is(':checked')) {
            $('#residency-address-editor-container').hide();
        }
        else {
            $('#residency-address-editor-container').show();
        }
    });
    $('#executive-residency-address-same-as-setup').find('input[type=checkbox]').change(function () {
        var checkbox = $(this);
        if (checkbox.is(':checked')) {
            $('#executive-residency-address-editor-container').hide();
        }
        else {
            $('#executive-residency-address-editor-container').show();
        }
    });
    $('#company-address-same-as-setup').find('input[type=checkbox]').change(function () {
        var checkbox = $(this);
        if (checkbox.is(':checked')) {
            $('#company-address-editor-container').hide();
        }
        else {
            $('#company-address-editor-container').show();
        }
    });
    $('#billing-address-same-as-setup').find('input[type=checkbox]').trigger('change');
    $('#residency-address-same-as-setup').find('input[type=checkbox]').trigger('change');
    $('#executive-residency-address-same-as-setup').find('input[type=checkbox]').trigger('change');
    $('#company-address-same-as-setup').find('input[type=checkbox]').trigger('change');
}

//subscription view

// username toggle
function SubscriptionViewSetupUsernameTypeToggle() {
    $('#username-mode-toggle').click(function () {
        $('#manual-username').find('input').val('');
        $('#automatic-username').toggle();
        $('#manual-username').toggle();
    });
    if ($('#manual-username').find('input').val() != '') {
        $('#automatic-username').toggle();
        $('#manual-username').toggle();
    }
}

//  -------------- domain changes functions -------------------------
function updateTariffList(selectedTariffValue, selectedBillingPeriod) {
    var domainInput = $('#domain-container').find('div.select-list-wrapper').find('input[type="hidden"]');
    var tariffWrapper = $('#tariff-container').find('div.select-list-wrapper');
    var tariffInput = tariffWrapper.find('input[type="hidden"]');
    var billingPeriodWrapper = $('#billing-period-container').find('div.select-list-wrapper');
    var billingPeriodInput = billingPeriodWrapper.find('input[type="hidden"]');
    var billingPeriodPresetValue = billingPeriodInput.val();
    var fetchUrl = $('#tariff-container input.fetch-url').val();

    $('#tariff-check').attr('class', '').addClass('check-name-loading');
    if (domainInput.val() != '') {
        FillSelectListFromUrl(tariffWrapper, fetchUrl, { id: domainInput.val() }, function (status) {
            if (status = 'success') {
                $('#tariff-check').attr('class', '').addClass('check-name-valid');
                updateBillingPeriodList(selectedBillingPeriod);
            }
        }, selectedTariffValue);
    }
    else {
        ClearSelectListItems(tariffWrapper);
        $('#tariff-check').attr('class', '').addClass('check-name-invalid');
    }
}

function updateBillingPeriodList(selectedBillingPeriod) {
    var domainInput = $('#domain-container').find('div.select-list-wrapper').find('input[type="hidden"]');
    var tariffWrapper = $('#tariff-container').find('div.select-list-wrapper');
    var tariffInput = tariffWrapper.find('input[type="hidden"]');
    var billingPeriodWrapper = $('#billing-period-container').find('div.select-list-wrapper');
    var billingPeriodInput = billingPeriodWrapper.find('input[type="hidden"]');
    var billingPeriodPresetValue = billingPeriodInput.val();
    var fetchUrl = $('#billing-period-container input.fetch-url').val();

    $('#billing-period-check').attr('class', '').addClass('check-name-loading');
    if (tariffInput.val() != '') {
        FillSelectListFromUrl(billingPeriodWrapper, fetchUrl, { id: tariffInput.val() }, function (status, items) {
            if (status = 'success') {
                $('#billing-period-check').attr('class', '').addClass('check-name-valid');
                if (items != null) {
                    if (items.length == 0) {
                        billingPeriodWrapper.hide();
                    }
                    else {
                        billingPeriodWrapper.fadeIn(200);
                    }
                }
            }
        }, selectedBillingPeriod);
    }
    else {
        ClearSelectListItems(billingPeriodWrapper);
        $('#billing-period-check').attr('class', '').addClass('check-name-invalid');
    }
}

function updateUIForCredentials() {
    $('#telekom-info').hide();
    $('#domain-credentials-error').hide();
    $('#infrastructure-check-results').html('');
    var validRegTypes = $('#valid-registration-types').val().split(',');
    var domainInput = $('#domain-container').find('div.select-list-wrapper').find('input[type="hidden"]');
    var registrationTypeInput = $('input[name$=RegistrationType]');
    var checkDomainCredentialsUrl = $('#domain-credentials-check-url').val();

    if (domainInput.val() != '' && $.inArray(registrationTypeInput.val(), validRegTypes) != -1) {
        $.ajax(checkDomainCredentialsUrl, {
            data: { id: domainInput.val() },
            method: 'POST',
            complete: function (data, status) {
                if (status == 'success') {
                    if (data.responseJSON.FoundCredentials) {
                        $('#telekom-info').show();
                    }
                }
                else {
                    $('#domain-credentials-error').show();
                }
            }
        })
    }
}
// --------------------------------------------------------------------------------------
// set customer tariff programatically
function SetTariffProgramatically(domainId, tariffId, billingPeriod) {
    var domainInputWrapper = $('#domain-container').find('div.select-list-wrapper');
    var domainInput = domainInputWrapper.find('input[type="hidden"]');
    domainInput.off('click', updateTariffList);
    selectOption(domainId, domainInputWrapper);
    domainInput.on('click', updateTariffList);
    updateTariffList(tariffId, billingPeriod);
}
// domain changes
function SubscriptionViewSetupDomainChanges(packetListForDomainUrl, packetListInputPrefix) {
    var domainInput = $('#domain-container').find('div.select-list-wrapper').find('input[type="hidden"]');
    var tariffWrapper = $('#tariff-container').find('div.select-list-wrapper');
    var tariffInput = tariffWrapper.find('input[type="hidden"]');
    var billingPeriodWrapper = $('#billing-period-container').find('div.select-list-wrapper');
    var billingPeriodInput = billingPeriodWrapper.find('input[type="hidden"]');
    var billingPeriodPresetValue = billingPeriodInput.val();
    // domain -> tariff -> billing period
    domainInput.change(updateTariffList);
    tariffInput.change(function () {
        updateBillingPeriodList();
    });
    //var updateTariffList = function (selectedTariffValue, selectedBillingPeriod) {
    //    $('#tariff-check').attr('class', '').addClass('check-name-loading');
    //    if (domainInput.val() != '') {
    //        FillSelectListFromUrl(tariffWrapper, domainTariffsUrl, { id: domainInput.val() }, function (status) {
    //            if (status = 'success') {
    //                $('#tariff-check').attr('class', '').addClass('check-name-valid');
    //                updateBillingPeriodList(selectedBillingPeriod);
    //            }
    //        }, selectedTariffValue);
    //    }
    //    else {
    //        ClearSelectListItems(tariffWrapper);
    //        $('#tariff-check').attr('class', '').addClass('check-name-invalid');
    //    }
    //};
    //var updateBillingPeriodList = function (selectedBillingPeriod) {
    //    $('#billing-period-check').attr('class', '').addClass('check-name-loading');
    //    if (tariffInput.val() != '') {
    //        FillSelectListFromUrl(billingPeriodWrapper, billingPeriodsUrl, { id: tariffInput.val() }, function (status, items) {
    //            if (status = 'success') {
    //                $('#billing-period-check').attr('class', '').addClass('check-name-valid');
    //                if (items != null) {
    //                    if (items.length == 0) {
    //                        billingPeriodWrapper.hide();
    //                    }
    //                    else {
    //                        billingPeriodWrapper.fadeIn(200);
    //                    }
    //                }
    //            }
    //        }, selectedBillingPeriod);
    //    }
    //    else {
    //        ClearSelectListItems(billingPeriodWrapper);
    //        $('#billing-period-check').attr('class', '').addClass('check-name-invalid');
    //    }
    //};
    updateTariffList(tariffInput.val(), billingPeriodPresetValue);

    // domain -> tt-credentials
    domainInput.change(updateUIForCredentials);
    //var updateUIForCredentials = function () {
    //    $('#telekom-info').hide();
    //    $('#domain-credentials-error').hide();
    //    $('#infrastructure-check-results').html('');
    //    var validRegTypes = $('#valid-registration-types').val().split(',');
    //    if (domainInput.val() != '' && $.inArray(registrationTypeInput.val(), validRegTypes) != -1) {
    //        $.ajax(checkDomainCredentialsUrl, {
    //            data: { id: domainInput.val() },
    //            method: 'POST',
    //            complete: function (data, status) {
    //                if (status == 'success') {
    //                    if (data.responseJSON.FoundCredentials) {
    //                        $('#telekom-info').show();
    //                    }
    //                }
    //                else {
    //                    $('#domain-credentials-error').show();
    //                }
    //            }
    //        })
    //    }
    //};
    updateUIForCredentials();

    // domain -> packet selection
    domainInput.change(function () {
        updatePacketSelection();
    });
    var updatePacketSelection = function () {
        var loader = new AjaxLoader('#packet-selection-container', false);
        loader.SetCallback(function () {
            var jsonContainer = $('#packet-selection-container').find('div.json-source');
            if (jsonContainer.length > 0) {
                var parsedTariffs = JSON.parse(jsonContainer.text());
                var xdslTypeSelector = $('#packet-selection-container').find('div.xdsl-type-selection');
                var xdslTypeSelectorInput = xdslTypeSelector.find('input[type=hidden][name]');
                var speedSelector = $('#packet-selection-container').find('div.speed-selection');
                var speedSelectorInput = speedSelector.find('input[type=hidden][name]');
                var tariffSelector = $('#packet-selection-container').find('div.tariff-selection');
                var tariffSelectorInput = tariffSelector.find('input[type=hidden][name]');

                tariffSelectorInput.change(function () {
                    var packetCodeInput = $('input.packet-code-hidden');
                    var tariffCodeInput = $('input.tariff-code-hidden');
                    if (tariffSelectorInput.val() == '') {
                        packetCodeInput.val('');
                        tariffCodeInput.val('');
                    }
                    else {
                        var partedValue = tariffSelectorInput.val().split(',');
                        if (partedValue.length != 2) {
                            packetCodeInput.val('');
                            tariffCodeInput.val('');
                        }
                        else {
                            packetCodeInput.val(partedValue[1]);
                            tariffCodeInput.val(partedValue[0]);
                        }
                    }
                });

                speedSelectorInput.change(function () {
                    var parentValue = xdslTypeSelectorInput.val();
                    var currentValue = speedSelectorInput.val();
                    if (currentValue == "") {
                        tariffSelector.find('div.list-option:not([value=""])').remove();
                        selectOption('', tariffSelector);
                        return;
                    }

                    for (var i = 0; i < parsedTariffs.length; i++) {
                        if (parsedTariffs[i].InfrastructureType == parseInt(parentValue)) {
                            for (var j = 0; j < parsedTariffs[i].Tariffs.length; j++) {
                                if (parsedTariffs[i].Tariffs[j].Speed.SpeedCode == parseInt(currentValue)) {
                                    var tariffOptions = tariffSelector.find('div.options-container').children().first();
                                    tariffOptions.find('div.list-option:not([value=""])').remove();
                                    for (var k = 0; k < parsedTariffs[i].Tariffs[j].Tariffs.length; k++) {
                                        tariffOptions.append('<div class="list-option" value="' + parsedTariffs[i].Tariffs[j].Tariffs[k].TariffCode + ',' + parsedTariffs[i].Tariffs[j].Tariffs[k].PacketCode + '">' + parsedTariffs[i].Tariffs[j].Tariffs[k].TariffName + '<div>');
                                    }
                                    SetupSelectLists(tariffSelector.parent());
                                    selectOption('', tariffSelector);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                });

                xdslTypeSelectorInput.change(function () {
                    var currentValue = xdslTypeSelectorInput.val();
                    if (currentValue == "") {
                        speedSelector.find('div.list-option:not([value=""])').remove();
                        tariffSelector.find('div.list-option:not([value=""])').remove();
                        selectOption('', speedSelector);
                        return;
                    }
                    for (var i = 0; i < parsedTariffs.length; i++) {
                        if (parsedTariffs[i].InfrastructureType == parseInt(currentValue)) {
                            var speedOptions = speedSelector.find('div.options-container').children().first();
                            speedOptions.find('div.list-option:not([value=""])').remove();
                            for (var j = 0; j < parsedTariffs[i].Tariffs.length; j++) {
                                speedOptions.append('<div class="list-option" value="' + parsedTariffs[i].Tariffs[j].Speed.SpeedCode + '">' + parsedTariffs[i].Tariffs[j].Speed.SpeedName + '<div>');
                            }
                            SetupSelectLists(speedSelector.parent());
                            selectOption('', speedSelector);
                            break;
                        }
                    }
                });
            }
            SetupCheckbuttons('#packet-selection-container');
        });
        loader.Load(packetListForDomainUrl, { domainId: domainInput.val(), prefix: packetListInputPrefix }, 'POST');
    };
}

// infrastructure check
function SubscriptionViewSetupInfrastructureCheck(availabilityCheckUrl) {
    var domainInput = $('#domain-container').find('div.select-list-wrapper').find('input[type="hidden"]');
    $('#infrastructure-check-button').click(function () {
        var availabilityLoader = new AjaxLoader('#infrastructure-check-results', false);
        availabilityLoader.Load(availabilityCheckUrl, { domainId: domainInput.val(), BBK: $('#setup-address').find('input[type="hidden"][name$=".ApartmentID"]').val() }, 'POST');
    });
}

// telekom info mode toggle
function SubscriptionViewSetupTelekomInfoModeToggle() {
    $('#telekom-info-mode-toggle').click(function () {
        $('.automatic-telekom-info').toggle();
        $('.manual-telekom-info').toggle();

    });
    if ($('.manual-telekom-info').find('input[name]:not([value=""])').length > 0) {
        $('#telekom-info-mode-toggle').trigger('click');
    }
}

// reference no toggle
function SubscriptionViewSetupReferralDiscount() {
    $('#reference-no-input-container').find('input[name]').on('input', function () {
        var refInput = $(this);
        if (refInput.val().length > 0) {
            $('.referral-discount-related').show();
        }
        else {
            $('.referral-discount-related').hide();
        }
    });
    $('#reference-no-input-container').find('input[name]').trigger('input');
}

// registration type select
function SubscriptionViewSetupRegistrationType(validationUrl, newRegisterId, transitionId, transferId) {
    var transferringSubscriberDisplayInput = $('input[name$=TransferringSubscriptionNo]');
    var transferringSubscriberValueInput = $('input[name$=TransferringSubscriptionID]');
    var validationMark = $('#transferring-subscription-check');
    var checkButton = $('input.transferring-subscription-check');
    var validationContainer = $('#transferring-subscription-validation-message-container');
    var foundNameContainer = $('#transferring-subscription-found-name');
    var setupAddressBBKInput = $('#setup-address input.bbk-input');
    var setupAddressBBKFillButton = $('#setup-address input.bbk-fetch-button');
    var setupAddressPostalCodeInput = $('#setup-address input[name$=".PostalCode"]');
    var setupAddressFloorInput = $('#setup-address input[name$=".Floor"]');
    var domainInput = $('#domain-container').find('div.select-list-wrapper').find('input[type="hidden"]');

    $('input[name$=RegistrationType]').change(function () {
        var regTypeInput = $(this);
        var currentValue = parseInt(regTypeInput.val());

        if (currentValue == transferId) {
            $('tr.registration-type-transfer').show();
        }
        else {
            $('tr.registration-type-transfer').hide();
        }
        if (currentValue == transitionId) {
            $('#transition-telekom-info').show();
        }
        else {
            $('#transition-telekom-info').hide();
        }

        updateUIForCredentials();
    });

    $('input[name$=RegistrationType]').trigger('change');

    checkButton.click(function () {
        if (transferringSubscriberDisplayInput.val().trim().length > 0) {
            checkButton.hide();
            validationMark.removeClass();
            validationMark.addClass('check-name-loading');

            $.ajax({
                url: validationUrl,
                data: {
                    transferringSubscriptionNo: transferringSubscriberDisplayInput.val()
                },
                method: 'POST',
                complete: function (data, status) {
                    if (status == 'success') {
                        var validationResponse = data.responseJSON;
                        if (validationResponse.IsValid) {
                            validationMark.removeClass();
                            validationMark.addClass('check-name-valid');
                            validationContainer.text('');
                            foundNameContainer.text(validationResponse.SubName);
                            transferringSubscriberValueInput.val(validationResponse.SubId);
                            setupAddressBBKInput.val(validationResponse.BBK);
                            setupAddressBBKFillButton.trigger('click');
                            setupAddressPostalCodeInput.val(validationResponse.PostalCode);
                            setupAddressFloorInput.val(validationResponse.Floor);
                            SetTariffProgramatically(validationResponse.DomainID, validationResponse.TariffID, validationResponse.BillingPeriod);
                        }
                        else {
                            validationMark.removeClass();
                            validationMark.addClass('check-name-invalid');
                            validationContainer.text(validationResponse.ValidationMessage);
                            foundNameContainer.text('');
                            transferringSubscriberValueInput.val('');
                            setupAddressPostalCodeInput.val('');
                            setupAddressFloorInput.val('');
                        }
                    }
                    else {
                        validationMark.removeClass();
                        validationMark.addClass('check-name-duplicate');
                        foundNameContainer.text('');
                        transferringSubscriberValueInput.val('');
                        setupAddressPostalCodeInput.val('');
                        setupAddressFloorInput.val('');
                    }
                    checkButton.show();
                }
            });
        }
    });

    checkButton.trigger('click');
}