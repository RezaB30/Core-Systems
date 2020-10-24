function SetupTransferLimits(containerId) {
    setupTransferRates(containerId);

    var containers = $(containerId);

    containers.each(function () {
        var controls = $(this).find(".rate-limit-wrapper");

        controls.each(function () {
            var currentWrapper = $(this);
            var totalValue = currentWrapper.find(".rate-limit-hidden").first();
            var subValues = currentWrapper.find(".transfer-rate-hidden");
            var timeValues = currentWrapper.find(".rate-limit-time");

            if (subValues.length == 6 && timeValues.length == 2) {
                subValues.change(function () {
                    setValue();
                });
                timeValues.change(function () {
                    setValue();
                });
                setValue();
            }

            function setValue() {
                var rx = $(subValues[1]).val();
                var tx = $(subValues[0]).val();
                var rxBurst = $(subValues[3]).val();
                var txBurst = $(subValues[2]).val();
                var rxBurstThreshold = $(subValues[5]).val();
                var txBurstThreshold = $(subValues[4]).val();
                var rxBurstTime = $(timeValues[1]).val();
                var txBurstTime = $(timeValues[0]).val();

                if (rx.length > 0 && tx.length > 0) {
                    totalValue.val(rx + "/" + tx);

                    if (rxBurst.length > 0 && txBurst.length > 0) {
                        totalValue.val(totalValue.val() + " " + rxBurst + "/" + txBurst);

                        if (rxBurstThreshold.length > 0 && txBurstThreshold.length > 0) {
                            totalValue.val(totalValue.val() + " " + rxBurstThreshold + "/" + txBurstThreshold);

                            if (rxBurstTime.length > 0 && txBurstTime.length > 0) {
                                totalValue.val(totalValue.val() + " " + rxBurstTime + "/" + txBurstTime);
                            }
                        }
                    }
                }
            }
        });
    });
}