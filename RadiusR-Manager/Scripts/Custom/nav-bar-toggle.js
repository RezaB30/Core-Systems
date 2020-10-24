$('div.nav-bar-toggle').click(function () {
    var toggleButton = $(this);
    var navBar = $('div.nav-bar');
    var pageContent = $('div.page-contents');
    navBar.toggleClass('collapsed');
    toggleButton.toggleClass('collapsed');
    pageContent.toggleClass('collapsed');
});