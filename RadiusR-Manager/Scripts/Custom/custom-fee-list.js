function SetupCustomFeeList(containerId) {
    var container = $(containerId);

    var modelNameContainer = container.find('#custom-fee-model-name');
    var modelName = modelNameContainer.val();
    var form = container.closest('form');
    var table = container.closest('table');
    var sampleRows = container.find('tr.custom-fee-sample-row');
    var allRemoveButtons = container.find('.remove-button');

    //// clear values in sample
    //sampleRows.find('input[type!=button]').val('');

    // form submit
    form.submit(function () {
        container.find('.custom-fee-sample-row').remove();
        rearrangeNames();
    });

    // remove buttons click
    allRemoveButtons.click(function (e) { removeButton_click(e.currentTarget); });

    // remove button click
    function removeButton_click(target) {
        var currentRemoveButton = $(target);
        var currentRow = currentRemoveButton.closest('tr');
        var previousRow = currentRow.prev();
        currentRow.remove();
        previousRow.remove();
        rearrangeNames();
    }

    // add button click
    var addButton = container.find('.add-instance-button').last();

    addButton.click(function () {
        var lastRow = table.find('tr').last();
        lastRow.before(sampleRows.clone(false));
        var addedRows = lastRow.prev().add(lastRow.prev().prev());
        addedRows.fadeIn(200);
        addedRows.removeClass('custom-fee-sample-row')
        addedRows.find('input:visible').first().focus();

        setupNewRows(addedRows);
        rearrangeNames();
    });

    // setup new rows events
    function setupNewRows(rows) {
        var currentRows = $(rows);
        currentRows.each(function () {
            var currentRow = $(this);
            SetupSelectLists(currentRow);
            currentRow.find('.remove-button').click(function (e) { removeButton_click(e.currentTarget); });
        });
    }

    // rearrange input id and names
    function rearrangeNames() {
        var validRows = table.find('tr.custom-fee-row:not(.custom-fee-sample-row)');

        validRows.each(function (i) {
            var currentRow = $(this);
            var validInputs = currentRow.find('input[type!=button][name]');

            validInputs.each(function () {
                var currentInput = $(this);
                var currentName = currentInput.attr('name');
                var label = currentInput.closest('td').prev('td').find('label').first();
                var suffix = currentName.split('.').pop();
                currentInput.attr('name', modelName + '[' + Math.floor(i / 2) + '].' + suffix);
                currentInput.attr('id', modelName + '_' + Math.floor(i / 2) + '__' + suffix);
                label.attr('for', currentInput.attr('id'));
            });
        });
    }

    // rearrange names for the first load
    rearrangeNames();
}