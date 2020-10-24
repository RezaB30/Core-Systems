function SetupBillList(containerId) {
    var container = $(containerId);
    var payAllButton = container.find('.payment-button').first();
    var allReceipts = container.find('.bill-receipt-container');
    var form = payAllButton.closest('form');
    var allCheckBoxes = container.find('input[type=checkbox]');

    allReceipts.click(function () {
        var current = $(this);
        var checkbox = current.prevAll('input[type=checkbox]').first();
        current.toggleClass('selected');
        checkbox.prop('checked', current.is('.selected'));
    });

    allReceipts.keydown(function (event) {
        var current = $(this);
        if (event.which == 13 || event.which == 32) {
            current.trigger('click');
        }
    });

    payAllButton.click(function () {
        allReceipts.addClass('selected');
        allCheckBoxes.prop('checked', true);
        form.trigger('submit');
    });
}