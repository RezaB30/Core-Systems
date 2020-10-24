function SetupFeeTypeVariantLists(containerId) {
    var container = $(containerId);
    var lists = container.find('.fee-type-list-wrapper');
    lists.each(function () {
        var current = $(this);
        var modelNameContainer = container.find('#name_container');
        var modelName = modelNameContainer.val();
        var form = current.find('form');
        var table = current.find('table.input-table').first();
        var sampleRow = current.find('.fee-type-list-sample').first().find('tr').first();
        var allRemoveButtons = current.find('.remove-button');

        // clear values in sample
        sampleRow.find('input[type!=button]').val('');

        // form submit
        form.click(function () {
            form.submit(function () {
                rearrangeNames();
            });
        });

        // remove buttons click
        allRemoveButtons.each(function () {
            var currentRemoveButton = $(this);
            currentRemoveButton.click(function () {
                var currentRow = $(this).closest('tr');
                currentRow.remove();
                rearrangeNames();
            });
        });

        // add button click
        var addButton = current.find('.add-instance-button').first();

        addButton.click(function () {
            var lastRow = table.find('tr').last();
            lastRow.before(sampleRow.clone(true));
            lastRow.prev().fadeIn(200);
            lastRow.prev().find('input:visible').first().focus();

            rearrangeNames();
        });

        // rearrange input id and names
        function rearrangeNames() {
            var validRows = table.find('tr');

            validRows.each(function (i) {
                var currentRow = $(this);
                var validInputs = currentRow.find('input[type!=button]');

                validInputs.each(function () {
                    var currentInput = $(this);
                    var currentName = currentInput.attr('name');
                    var label = currentInput.closest('td').prev('td').find('label').first();
                    var suffix = currentName.split('.')[1];
                    currentInput.attr('name', modelName + '[' + i + '].' + suffix);
                    currentInput.attr('id', modelName + '_' + i + '__' + suffix);
                    label.attr('for', currentInput.attr('id'));
                });
            });
        }
    });
}