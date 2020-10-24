function SetupAccardeon(containerId) {
    var container = $(containerId);
    var titleButtons = container.find('.accardeon-title-button');
    var pages = container.find('.accardeon-page');

    titleButtons.each(function (i) {
        var currentButton = $(this);
        var hash = currentButton.attr('id');
        currentButton.click(function () {
            if (!currentButton.is('.selected')) {
                titleButtons.removeClass('selected');
                currentButton.addClass('selected');
                pages.has(':visible').slideUp(300);
                $(pages.get(i)).slideDown(300);

                if (typeof hash !== 'undefined')
                    window.location.hash = hash;
            }
        });
    });

    // go to hash page
    var urlHash = window.location.hash;
    if (typeof urlHash !== 'undefined') {
        $(urlHash).trigger('click');
    }
}