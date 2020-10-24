function SetupClearSearchs(containerId) {
    var clearSearchButtons = $(containerId).find('.clear-search');
    clearSearchButtons.each(function () {
        var current = $(this);
        var clearingInputs = current.closest('.input-table').find('input[type=text],input[type=checkbox],textarea,input[type=hidden][name]');
        current.click(function () {
            clearingInputs.val('');
            clearingInputs.prop('checked', false);
            clearingInputs.removeAttr('checked');
            clearingInputs.filter('textarea').text('');
            current.closest('form').submit();
        });
    });
}