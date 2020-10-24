function SetupRelatedSubscriptionsList(containerId) {
    var container = $(containerId);
    container.find('div.related-subscriptions-wrapper').each(function () {
        var currentWrapper = $(this);
        var mainButton = currentWrapper.find('div.main-button');
        var itemList = currentWrapper.find('div.list-container');
        currentWrapper.click(function (e) {
            itemList.toggleClass('open');
            e.stopPropagation();
        });
        currentWrapper.keydown(function (e) {
            if (e.keyCode == 13 || e.keyCode == 32) {
                currentWrapper.trigger('click');
            }
            else if (e.keyCode == 27) {
                itemList.removeClass('open');
            }
        });
        $(document).click(function () {
            itemList.removeClass('open');
        });
    });
}