function SetupTelekomTariffSelector(containerId) {
    $(containerId).find('div.telekom-tariff-editor-wrapper').each(function () {
        var currentWrapper = $(this);

        var jsonContainer = currentWrapper.find('div.json-source');
        if (jsonContainer.length > 0) {
            var parsedTariffs = JSON.parse(jsonContainer.text());
            var xdslTypeSelector = currentWrapper.find('div.xdsl-type-selection');
            var xdslTypeSelectorInput = xdslTypeSelector.find('input[type=hidden][name]');
            var speedSelector = currentWrapper.find('div.speed-selection');
            var speedSelectorInput = speedSelector.find('input[type=hidden][name]');
            var tariffSelector = currentWrapper.find('div.tariff-selection');
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
        SetupCheckbuttons(currentWrapper);
    });
}