function SetupCitiesTree(containerId, samplePaletteId, hasProvince, cityProvinceId, editId) {
    var container = $(containerId);
    var samplePalette = $(samplePaletteId);
    var addProvinceButtons = container.find('input.add-province-button');
    var addCityButtons = container.find('input.add-city-button');
    var lists = container.find('ul.city-list');
    var deleteButtons = container.find('div.city-list-round-button.delete-button');
    var editProvinceButtons = container.find('div.province-title div.city-list-round-button.edit-button');
    var editCityButtons = container.find('div.city-list-title:not(.province-title) div.city-list-round-button.edit-button');

    lists.each(function () {
        var currentList = $(this);
        var provinceTitles = currentList.find('div.province-title');

        provinceTitles.each(function () {
            var currentTitle = $(this);
            var currentToggle = currentTitle.find('div.title-toggle');
            currentToggle.click(function () {
                currentToggle.toggleClass('open');
                currentTitle.closest('li').find('ul').slideToggle(200);
            });
        });
    });

    deleteButtons.click(function () {
        var currentButton = $(this);
        currentButton.closest('form').submit();
    });
    deleteButtons.keydown(function (e) {
        if (e.which == 13 || e.which == 32) {
            $(this).trigger('click');
        }
    });

    // samples
    var newProvinceSample = samplePalette.find('.city-list-new-province-sample');
    var editProvinceSample = samplePalette.find('.city-list-edit-province-sample');
    var newCitySample = samplePalette.find('.city-list-new-city-sample');
    var editCitySample = samplePalette.find('.city-list-edit-city-sample');
    // ---------

    addProvinceButtons.click(function () {
        var currentButton = $(this);
        var currentContainer = currentButton.parent();

        currentContainer.children().hide();
        currentButton.before('<div class="city-list-editor" style="display: none;">' + newProvinceSample.html() + '</div>');
        var currentEditor = currentContainer.find('div.city-list-editor');
        setupEditorSample(currentEditor);
        currentEditor.fadeIn(500);
        currentEditor.find('input:not([type=hidden])').first().focus();
    });

    editProvinceButtons.click(function () {
        var currentButton = $(this);
        var currentContainer = currentButton.closest('div.province-title');

        currentContainer.children().hide();
        currentContainer.append('<div class="city-list-editor" style="display: none; margin-left: 2em;">' + editProvinceSample.html() + '</div>');
        var currentEditor = currentContainer.find('div.city-list-editor');
        currentEditor.find('.city-list-code-editor').val(currentContainer.find('div.title-code').text());
        currentEditor.find('.city-list-name-editor').val(currentContainer.find('div.title-name').text());
        setupEditorSample(currentEditor);
        currentEditor.fadeIn(500);
        currentEditor.find('input:not([type=hidden])').first().focus();
    });

    addCityButtons.click(function () {
        var currentButton = $(this);
        var currentProvinceId = currentButton.attr('data-id');
        var currentContainer = currentButton.parent();

        currentContainer.children().hide();
        currentButton.before('<div class="city-list-editor" style="display: none;">' + newCitySample.html() + '</div>');
        var currentEditor = currentContainer.find('div.city-list-editor');
        currentEditor.find('form').prepend('<input name="id" type="hidden" value="' + currentProvinceId + '" />');
        setupEditorSample(currentEditor);
        currentEditor.fadeIn(500);
        currentEditor.find('input:not([type=hidden])').first().focus();
    });

    editCityButtons.click(function () {
        var currentButton = $(this);
        var currentContainer = currentButton.closest('div.city-list-title:not(.province-title)');

        currentContainer.children().hide();
        currentContainer.append('<div class="city-list-editor" style="display: none; margin-left: 2em;">' + editCitySample.html() + '</div>');
        var currentEditor = currentContainer.find('div.city-list-editor');
        currentEditor.find('.city-list-code-editor').val(currentContainer.find('div.title-code').text());
        currentEditor.find('.city-list-name-editor').val(currentContainer.find('div.title-name').text());
        setupEditorSample(currentEditor);
        currentEditor.fadeIn(500);
        currentEditor.find('input:not([type=hidden])').first().focus();
    });

    // shared function
    function setupEditorSample(currentContainer) {
        currentContainer = $(currentContainer);
        var cancelButton = currentContainer.find('div.cancel-button');
        var acceptButton = currentContainer.find('div.accept-button');

        currentContainer.keydown(function (e) {
            if (e.which == 27)
                cancelButton.trigger('click');
        });

        cancelButton.click(function () {
            currentContainer.siblings().show();
            currentContainer.remove();
        });
        cancelButton.keydown(function (e) {
            if (e.which == 13 || e.which == 32)
                cancelButton.trigger('click');
        });

        acceptButton.click(function () {
            acceptButton.closest('form').submit();
        });
        acceptButton.keydown(function (e) {
            if (e.which == 13 || e.which == 32)
                acceptButton.trigger('click');
        });

        currentContainer.find('input').keydown(function (e) {
            if (e.which == 13) {
                e.preventDefault();
                $(this).closest('form').submit();
            }
        });
    }

    // if has province
    if (hasProvince) {
        if (editId) {
            container.find('div.province-title div.edit-button[data-id="' + editId + '"]').trigger('click');
        }
        else {
            addProvinceButtons.trigger('click');
        }
    }
    // if has city
    if (cityProvinceId != null) {
        if (editId) {
            container.find('div.title-toggle[data-id=' + cityProvinceId + ']').trigger('click');
            container.find('div.city-list-title:not(.province-title) div.city-list-round-button.edit-button[data-id=' + editId + ']').trigger('click');
        }
        else {
            container.find('div.title-toggle[data-id=' + cityProvinceId + ']').trigger('click');
            container.find('input.add-city-button[data-id=' + cityProvinceId + ']').trigger('click');
        }
    }
}