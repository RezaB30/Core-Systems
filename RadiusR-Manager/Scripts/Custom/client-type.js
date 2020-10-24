function SetupClientTypeSelection(valueContainerId, valueContainerIdPair) {
    var referenceInput = $(valueContainerId);

    function updateVisuals() {
        var togoVisible = null;
        for (var i = 0; i < valueContainerIdPair.length; i++) {
            if (valueContainerIdPair[i].value == referenceInput.val())
                togoVisible = valueContainerIdPair[i].containerId;
            else
                $(valueContainerIdPair[i].containerId).hide();
        }
        $(togoVisible).fadeIn(200);
    }

    referenceInput.change(updateVisuals);

    updateVisuals();
}