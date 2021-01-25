$('div.nav-bar-toggle').click(function () {
    var toggleButton = $(this);
    var navBar = $('div.nav-bar');
    var pageContent = $('div.page-contents');
    var footerWrapper = $('div.footer-wrapper');
    navBar.toggleClass('collapsed');
    toggleButton.toggleClass('collapsed');
    pageContent.toggleClass('collapsed');
    footerWrapper.toggleClass('collapsed');
});