function SetupQuotaTypeChange(containerId, quotaInputName) {
    var container = $(containerId);
    var quotaInput = container.find('input[name="' + quotaInputName + '"]');
    var typeValue = quotaInput.val();

    quotaInput.change(function () {
        setCells(quotaInput.val());
    });

    function setCells(currentValue) {
        var allRelatedCells = container.find("[class^='quota-type-']");
        var validCells = allRelatedCells.filter('.quota-type-' + currentValue);
        allRelatedCells.hide();
        validCells.fadeIn();
    }

    setCells(typeValue);
}