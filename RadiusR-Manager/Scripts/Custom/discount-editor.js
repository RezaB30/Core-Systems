function SetupRecurringDiscountEditor(containerId, staticDiscountId, percentageDiscountId, feeBasedDiscountId, billBasedDiscountId) {
    var container = $(containerId);
    var discountTypeInput = container.find('input[name="DiscountType"],input[name$=".DiscountType"]');
    var applicationTypeInput = container.find('input[name="ApplicationType"],input[name$=".ApplicationType"]');

    discountTypeInput.change(function () {
        var discountType = $(this).val();
        if (discountType == staticDiscountId) {
            $('.discount-amount-related').show();
            $('.discount-percentage-amount-related').hide();
        }
        else if (discountType == percentageDiscountId) {
            $('.discount-amount-related').hide();
            $('.discount-percentage-amount-related').show();
        }
        else {
            $('.discount-amount-related').hide();
            $('.discount-percentage-amount-related').hide();
        }
    });

    applicationTypeInput.change(function () {
        var applicationType = $(this).val();
        if (applicationType == feeBasedDiscountId) {
            $('.discount-fee-type-related').show();
        }
        else {
            $('.discount-fee-type-related').hide();
        }
    });

    applicationTypeInput.trigger('change');
    discountTypeInput.trigger('change');
}