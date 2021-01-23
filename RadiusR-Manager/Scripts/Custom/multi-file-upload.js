function SetupMultiFileUploads(containerId) {
    var container = $(containerId);
    container.find('div.multi-file-upload-wrapper').each(function () {
        var currentContainer = $(this);
        var sampleContainer = currentContainer.find('div.sample-container');
        var addInstanceButton = currentContainer.find('input.add-instance-button');
        var rowsContainer = currentContainer.find('div.rows-container');

        addInstanceButton.click(function () {
            rowsContainer.append(sampleContainer.children().clone(false));
            rowsContainer.show();
            var addedRow = rowsContainer.find('div.instance-row').last();
            SetupFileUploads(addedRow);
            addedRow.find("input.delete-button").click(function () {
                $(this).closest('div.instance-row').remove();
                if (rowsContainer.children('div.instance-row').length <= 0) {
                    rowsContainer.hide();
                }
            });
        });

        currentContainer.closest('form').submit(function () {
            sampleContainer.remove();
        });
    });
}