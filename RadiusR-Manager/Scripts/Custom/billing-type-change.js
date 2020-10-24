function SetupBillingTypeChange(containerId, billingInputName) {
    var container = $(containerId);
    var quotaInput = container.find('input[name="' + billingInputName + '"]');
    var typeValue = quotaInput.val();

    quotaInput.change(function () {
        setCells(quotaInput.val());
    });

    function setCells(currentValue) {
        var allRelatedCells = container.find("[class^='billing-type-']");
        var validCells = allRelatedCells.filter('.billing-type-' + currentValue);
        allRelatedCells.hide();
        validCells.fadeIn();
    }

    setCells(typeValue);
}