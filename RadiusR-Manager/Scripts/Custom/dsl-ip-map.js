function SetupDSLIPMap(containerId) {
    let container = $(containerId);
    container.find('.ip-map-container').each(function () {
        let current = $(this);
        let modelName = current.find('input[name=prefixName]').val();
        let sampleRow = current.find('tr.ip-map-list-sample-row');
        let addButton = current.find('input.add-instance-button');
        let deleteButtons = current.find('input.remove-button');

        deleteButtons.click(function () {
            let current = $(this);
            let currentRow = current.closest('tr.ip-map-list-row');
            currentRow.remove();
        });

        addButton.click(function () {
            let clone = sampleRow.clone(true);
            clone.removeClass('ip-map-list-sample-row');
            addButton.closest('tr.ip-map-list-row').before(clone);
        });

        current.closest('form').submit(function () {
            sampleRow.remove();
            addButton.closest('tr.ip-map-list-row').remove();
            current.find('input.remove-button').remove();
            current.find('tr.ip-map-list-row input[name]').each(function (index) {
                let currentInput = $(this);
                let currentName = currentInput.attr('name');
                let nameArray = currentName.split('.');
                let suffix = nameArray.pop();
                //let prefix = nameArray.toString();
                //if (prefix.indexOf('[') > 0)
                //    prefix = prefix.substring(0, prefix.lastIndexOf('['));
                currentInput.attr('name', modelName + '[' + index + '].' + suffix);
                currentInput.attr('id', modelName + '_' + index + '__' + suffix);
            });
        });
    });
}