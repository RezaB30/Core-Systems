function SetupAddressDetails(containerId) {
    var container = $(containerId);
    container.find('div.address-details-container').each(function () {
        var currentContainer = $(this);
        var currentToggle = currentContainer.find('div.address-details-toggle').click(function () {
            currentContainer.toggleClass('closed');
        });
    });
}