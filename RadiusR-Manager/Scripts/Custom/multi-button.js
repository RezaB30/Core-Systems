function SetupMultiButtons(containerId) {
    var multiButtons = $(containerId).find('.multi-button-wrapper');
    multiButtons.each(function () {
        var current = $(this);
        var topButton = current.find('.multi-button-item-top').first();
        var optionsContainer = current.find('.multi-button-options').first();
        var options = optionsContainer.find('.multi-button-item');
        var stateHidden = current.find('input[name="State"]').first();
        var form = current.find('form').first();

        current.click(function () {
            topButton.toggleClass('open');
            optionsContainer.toggleClass('open');
            optionsContainer.fadeToggle(200);
        });

        current.focusout(function () {
            optionsContainer.removeClass('open');
            topButton.removeClass('open');
            optionsContainer.fadeOut(200);
            options.removeClass('selected');
        });

        current.keydown(function (event) {
            switch (event.which) {
                case 38:
                    var selected = options.filter('.selected');
                    if (selected.length > 0) {
                        selected.prev().addClass('selected');
                        selected.removeClass('selected');
                    }
                    else {
                        options.last().addClass('selected');
                    }
                    break;
                case 40:
                    var selected = options.filter('.selected');
                    if (selected.length > 0) {
                        selected.next().addClass('selected');
                        selected.removeClass('selected');
                    }
                    else {
                        options.first().addClass('selected');
                    }
                    break;
                case 13:
                case 32:
                    if (optionsContainer.is(':visible')) {
                        var selected = options.filter('.selected');
                        if (selected.length > 0) {
                            selected.trigger('click');
                        }
                    }
                    else {
                        optionsContainer.trigger('click');
                        break;
                    }
                case 27:
                    current.trigger('focusout');
                default:
                    break;
            }
        });

        options.click(function () {
            var currentValue = $(this).attr('data-value');
            stateHidden.val(currentValue);
            form.submit();
        });
    });
}