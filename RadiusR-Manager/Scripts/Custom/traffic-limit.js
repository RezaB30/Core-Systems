function SetupTrafficLimits(containerId) {
    var containers = $(containerId);
    containers.each(function () {
        var controls = $(this).find(".traffic-limit-wrapper");
        controls.each(function () {
            var currentWrapper = $(this);
            var totalValue = currentWrapper.find(".traffic-limit-hidden").first();
            var suffixValue = currentWrapper.find(".select-list-wrapper").first().find("input[type=hidden]").first();
            var visibleText = currentWrapper.find(".traffic-limit-text").first();

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

                    totalValue.val(Number.parseInt(visibleText.val()) * Math.pow(1024, Number.parseInt(suffixValue.val())));
                    totalValue.trigger("change");
                }
            }
        });
    });
}