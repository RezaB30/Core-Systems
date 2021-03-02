function SetupMultiTextEditors(containerId) {
    let container = $(containerId);
    container.find('div.multi-text-editor-wrapper').each(function () {
        let currentWrapper = $(this);
        let sampleContainer = currentWrapper.find('div.multi-text-editor-sample');
        let sampleInputName = sampleContainer.find('input[name]').attr('name');
        let itemList = currentWrapper.find('ol.multiselect-orderedlist');
        let addRow = currentWrapper.find('div.multi-text-editor-add-container');
        let addInstanceButton = addRow.find('input.add-instance-button');
        let parentForm = currentWrapper.closest('form');

        addInstanceButton.click(function () {
            itemList.append('<li class="multiselect-input-row"></li>');
            let addedItem = itemList.find('li.multiselect-input-row').last();
            addedItem.html(sampleContainer.html());
            addedItem.find('input.remove-instance-button').click(function () {
                RemoveButtonClick($(this));
            });
        });

        itemList.find('input.remove-instance-button').each(function () {
            let currentButton = $(this);
            currentButton.click(function () {
                RemoveButtonClick($(this));
            });
        });

        parentForm.submit(function () {
            sampleContainer.remove();
            itemList.find('input[name=' + sampleInputName + ']').each(function () {
                if ($(this).val().trim() == '') {
                    $(this).closest('li.multiselect-input-row').remove();
                }
            });
        });
    });

    function RemoveButtonClick(button) {
        let current = $(button);
        current.closest('li.multiselect-input-row').remove();
    };
}