function setupPaymentButtons() {
    allWrappers = $('.payment-button-combo-wrapper');
    allWrappers.each(function () {
        var currentWrapper = $(this);
        var paymentTypeInput = currentWrapper.find('.select-list-wrapper').find('input[type=hidden]');
        var payButton = currentWrapper.find('input.payment-button');
        var payAndPrintButton = currentWrapper.find('input.print-button');

        paymentTypeInput.change(function () {
            var selectedValue = paymentTypeInput.val();
            if (selectedValue == "2" || selectedValue == "4" || selectedValue == "6") {
                payAndPrintButton.show();
            }
            else {
                payAndPrintButton.hide();
            }
            if (selectedValue == "1") {
                payButton.hide();
            }
            else {
                payButton.show();
            }
        });

        paymentTypeInput.trigger("change");

        // hide buttons after clicking to prevent double click
        function clickAndHide() {
            currentWrapper.children().hide();
            currentWrapper.append('<div class="online-status-loading"></div>');
            currentWrapper.addClass('online-status-container');
        }

        payButton.click(clickAndHide);
        payAndPrintButton.click(clickAndHide);
    });
}