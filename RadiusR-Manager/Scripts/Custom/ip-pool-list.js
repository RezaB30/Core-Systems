function SetupIPPoolList(containersId, rowCount) {
    var containers = $(containersId);

    containers.each(function () {
        var container = $(this);

        var modelNameContainer = container.find('#ip-map-model-name');
        var modelName = modelNameContainer.val();
        var form = container.closest('form');
        var sampleRows = container.find('tr.ip-map-list-sample-row');
        var allRemoveButtons = container.find('.remove-button');

        //// clear values in sample
        //sampleRows.find('input[type!=button]').val('');

        // form submit
        form.submit(function () {
            container.find('.ip-map-list-sample-row').remove();
            rearrangeNames();
        });

        // remove buttons click
        allRemoveButtons.click(function (e) { removeButton_click(e.currentTarget); });

        // remove button click
        function removeButton_click(target) {
            var currentRemoveButton = $(target);
            var currentRow = currentRemoveButton.closest('tr');
            currentRow.prev()
            //var markedRows = currentRow.add(currentRow.prev()).add(currentRow.prev().prev());
            var markedRows = currentRow.add(currentRow.prevUntil('.ip-map-list-bottom-row'));
            markedRows.remove();
            rearrangeNames();
        }

        // add button click
        var addButton = container.find('.add-instance-button').last();

        addButton.click(function () {
            var lastRow = container.find('.add-instance-button').closest('tr');
            lastRow.before(sampleRows.clone(false));
            //var addedRows = lastRow.prev().add(lastRow.prev().prev()).add(lastRow.prev().prev().prev());
            var addedRows = lastRow.prev().add(lastRow.prev().prevUntil('.ip-map-list-bottom-row'));
            addedRows.fadeIn(200);
            addedRows.removeClass('ip-map-list-sample-row')
            addedRows.find('input:visible').first().focus();

            setupNewRows(addedRows);
            rearrangeNames();
        });

        // setup new rows events
        function setupNewRows(rows) {
            var currentRows = $(rows);
            currentRows.each(function () {
                var currentRow = $(this);
                SetupCheckbuttons(currentRow);
                currentRow.find('.remove-button').click(function (e) { removeButton_click(e.currentTarget); });
            });
        }

        // rearrange input id and names
        function rearrangeNames() {
            var validRows = container.find('tr.ip-map-list-row:not(.ip-map-list-sample-row)');

            validRows.each(function (i) {
                var currentRow = $(this);
                var validInputs = currentRow.find('input[type!=button][name]');

                validInputs.each(function () {
                    var currentInput = $(this);
                    var currentName = currentInput.attr('name');
                    var label = currentInput.closest('td').prev('td').find('label').first();
                    var suffix = currentName.split('.').pop();
                    currentInput.attr('name', modelName + '[' + Math.floor(i / rowCount) + '].' + suffix);
                    currentInput.attr('id', modelName + '_' + Math.floor(i / rowCount) + '__' + suffix);
                    label.attr('for', currentInput.attr('id'));
                });
            });
        }

        // rearrange names for the first load
        rearrangeNames();
    });
}