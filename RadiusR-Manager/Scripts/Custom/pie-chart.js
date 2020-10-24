function SetupPieCharts() {
    var diagrams = $(".pie-diagram");
    diagrams.each(function () {
        var currentDiag = $(this);
        var tooltip = currentDiag.find(".diagram-tooltip");
        tooltip.hide();
        var slices = currentDiag.find(".chart-slice[title]");
        slices.each(function () {
            var currentSlice = $(this);
            currentSlice.hover(function (e) {
                tooltip.text(currentSlice.attr("title"));
                tooltip.stop(true, true);
                tooltip.fadeIn(200);
                tooltip.offset({ top: e.pageY - tooltip.outerHeight() - 20, left: e.pageX - (tooltip.outerWidth() / 2) });
            }, function () {
                tooltip.fadeOut(200);
            });
        });
    });
}