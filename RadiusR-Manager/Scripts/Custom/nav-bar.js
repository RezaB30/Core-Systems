function initializeNavbar() {
    var allTitles = $('.nav-menu-title');
    var allSubLists = $('.nav-list-sub');
    allTitles.click(function () {
        var currentTitle = $(this);
        if (currentTitle.hasClass('selected')) {
            currentTitle.removeClass('selected');
            currentTitle.parent().find('.nav-list-sub').stop(true, false).slideUp(200);
        }
        else {
            allTitles.removeClass('selected');
            currentTitle.addClass('selected');
            allSubLists.slideUp(200);
            currentTitle.parent().find('.nav-list-sub').stop(true, false).slideDown(200);
        }
    });
}

function initializeMobileMenu() {
    $('.m-menu').toggleClass('selected');
    $('div.nav-bar').toggleClass('selected');
    if ($('div.nav-bar').hasClass('selected')) {
        $('div.nav-bar').goTo();
    }
}