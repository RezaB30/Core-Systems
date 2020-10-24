function SetupSMSParameters(containerId) {
    var wrappers = $(containerId).find('div.sms-parameter-wrapper');
    wrappers.each(function () {
        var currentWrapper = $(this);
        var openButton = currentWrapper.prev('input.sms-parameter-open-button');
        var textArea = currentWrapper.closest('tr').find('textarea').first();
        var currentItems = currentWrapper.find('div.sms-parameter-item');

        currentWrapper.click(function () {
            currentWrapper.addClass('hidden');
            openButton.show();
        });

        openButton.click(function () {
            openButton.hide();
            currentWrapper.removeClass('hidden');
        });

        currentItems.click(function () {
            var currentItem = $(this);
            var startPos = textArea[0].selectionStart;
            textArea.val(textArea.val().substring(0, startPos) + currentItem.attr('data-value') + textArea.val().substring(startPos, textArea.val().length));
            textArea.focus();
        });
    });
}