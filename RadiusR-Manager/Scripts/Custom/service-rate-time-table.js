function SetupServiceRateTimeTable(containerId) {
    var container = $(containerId);
    var wrappers = container.find('div.rate-limit-time-partition-wrapper');
    wrappers.each(function () {
        var currentWrapper = $(this);
        var sampleRow = currentWrapper.find('div.sample-row');
        var addRow = currentWrapper.find('div.add-instance-row');
        var addButton = addRow.find('input.add-instance-button');
        var removeButtons = currentWrapper.find('div.remove-row input.delete-button');

        addButton.click(function () {
            addRow.before(sampleRow.clone(false));
            var addedRow = addRow.prev('div.sample-row');
            addedRow.removeClass('sample-row');
            addedRow.addClass('item-row');
            SetupSelectLists(addedRow);
            SetupTransferLimits(addedRow);
            setupRemoveButtons(addedRow);
        });

        function setupRemoveButtons(containerId2) {
            var currentButtons = $(containerId2).find('div.remove-row input.delete-button');
            currentButtons.click(function () {
                $(this).closest('div.item-row').remove();
            });
        }

        function setupForm() {
            var form = currentWrapper.closest('form');
            form.submit(function (e) {
                //e.preventDefault();
                var itemRows = currentWrapper.find('div.item-row');
                itemRows.each(function (index) {
                    var validInputs = $(this).find('input[name*=".sample."]');
                    validInputs.each(function () {
                        var currentName = $(this).attr('name');
                        $(this).attr('name', currentName.replace(/(\[\d\]\.)|(\.sample\.)/g, '[' + index + '].'));
                    });
                });
                currentWrapper.find('div.sample-row,div.add-instance-row,div.remove-row').remove();
                //if (!confirm('Yes?'))
                //    e.preventDefault();
            });
        }

        setupRemoveButtons(containerId);
        setupForm();
    });
}