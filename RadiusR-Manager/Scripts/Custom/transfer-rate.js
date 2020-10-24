function setupTransferRates(containerId) {
    var containers = $(containerId);
    containers.each(function () {
        var controls = $(this).find(".transfer-rate-wrapper");
        controls.each(function () {
            var currentWrapper = $(this);
            var totalValue = currentWrapper.find(".transfer-rate-hidden").first();
            var suffixValue = currentWrapper.find(".select-list-wrapper").first().find("input[type=hidden]").first();
            var visibleText = currentWrapper.find(".transfer-rate-text").first();

            visibleText.change(function () {
                changeValue();
            });

            suffixValue.change(function () {
                changeValue();
            });

            changeValue();

            function changeValue() {
                if (visibleText.val().length <= 0) {
                    totalValue.val("");
                }
                else {
                    totalValue.val(visibleText.val() + suffixValue.val());
                    totalValue.trigger("change");
                }
            }
        });
    });
}