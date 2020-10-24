function SetupLinearDiagrams(containerId) {
    var diagrams = $(containerId).find(".linear-diagram");
    diagrams.each(function () {
        var currentDiag = $(this);
        var tooltip = currentDiag.find(".diagram-tooltip");
        tooltip.hide();
        var diagramPins = currentDiag.find("circle[title]");
        diagramPins.each(function () {
            var currentPin = $(this);
            currentPin.hover(function () {
                tooltip.text(currentPin.attr("title"));
                var pos = currentPin.position();
                pos.top -= 10 + tooltip.outerHeight();
                pos.left -= (tooltip.outerWidth() / 2) - 5;
                tooltip.fadeIn(200);
                tooltip.offset(pos);
            }, function () {
                tooltip.fadeOut(200);
            });
        });
    });
}