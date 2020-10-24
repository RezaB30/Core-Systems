function SetupCategorySliders(containerId) {
    var container = $(containerId);
    var categories = container.find('.category-list-wrapper');

    categories.each(function () {
        var current = $(this);
        var categoryHeads = current.find('.category-list-head');

        categoryHeads.each(function (index) {
            var currentHead = $(this);
            var currentContents = currentHead.nextAll('.category-list-contents').first();
            var cookieName = current.attr('id') + "_" + index;

            if (getCookie(cookieName) != null) {
                currentContents.show();
            }

            currentHead.click(function () {

                if (currentContents.is(':visible')) {
                    removeCookie(cookieName);
                }
                else {
                    addCookie(cookieName, 1);
                }

                currentContents.slideToggle(200);
            });
        });

        categoryHeads.keydown(function (event) {
            var currentHead = $(this);
            if (event.which == 13 || event.which == 32) {
                currentHead.trigger('click');
            }
        });
    });
}