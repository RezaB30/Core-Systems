function SetupPhoneNoLists(containerId) {
    var container = $(containerId);
    var wrapperList = container.find('div.phone-no-list-wrapper');

    wrapperList.each(function () {
        var currentWrapper = $(this);
        var sampleRow = currentWrapper.find('div.sample-row');
        var addInstanceButton = currentWrapper.find('input.add-instance-button');
        var list = currentWrapper.find('ol');

        currentWrapper.find('input.remove-instance-button').click(function () {
            var currentButton = $(this);
            var currentListItem = currentButton.closest('li.multiselect-input-row');
            currentListItem.remove();
        });

        addInstanceButton.click(function () {
            list.append(sampleRow.children().clone(true));
        });

        currentWrapper.closest('form').submit(function () {
            sampleRow.remove();
            addInstanceButton.remove();
            currentWrapper.find('input.remove-instance-button').remove();
            list.find('li.multiselect-input-row').each(function (index) {
                currentListItem = $(this);
                currentListItem.find('input[name]').each(function () {
                    var currentInput = $(this);
                    currentInput.attr('name', currentInput.attr('name').replace(/(\[\d\]\.)|(\.sample\.)/, '[' + index + '].'));
                });
            });
        });

    });
}