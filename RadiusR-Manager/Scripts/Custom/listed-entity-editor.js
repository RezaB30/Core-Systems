function SetupListedEntityEditor(containerId, modelPrefix, sampleClass, addRowId, itemClass, deleteItemId, addItemId) {
    var container = $(containerId);
    var sample = $('.' + sampleClass);
    var addRow = $(addRowId);
    var addItemElement = $(addItemId);

    sample.find(deleteItemId).click(function () {
        var current = $(this);
        current.closest('.' + itemClass).remove();
        rearrengeList();
    });

    $('.' + itemClass).find(deleteItemId).click(function () {
        var current = $(this);
        current.closest('.' + itemClass).remove();
        rearrengeList();
    });

    addItemElement.click(function () {
        addRow.before(sample.clone(true));
        var newlyAdded = addRow.prev();
        newlyAdded.removeClass(sampleClass);
        newlyAdded.addClass(itemClass);
        rearrengeList();
    });

    function rearrengeList() {
        $('.' + itemClass).each(function (index) {
            var currentItem = $(this);
            var validInputs = currentItem.find('input[type!=button][name]');
            validInputs.each(function () {
                var currentInput = $(this);
                var name = currentInput.attr('name');
                var lastPart = name.split('.').pop();
                currentInput.attr('name', modelPrefix + '[' + index + '].' + lastPart);
            });
        });
    }

    container.closest('form').submit(function () {
        rearrengeList();
    });

    rearrengeList();
}