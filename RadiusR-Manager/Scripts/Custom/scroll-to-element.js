(function ($) {
    $.fn.goTo = function () {
        var additional = 0;
        var header = $(".header-bar").first();
        if (header.length > 0)
            additional = header.height() + 10;
        $('html, body').animate({
            scrollTop: ($(this).offset().top - additional) + 'px'
        }, 'fast');
        return this; // for chaining...
    }
})(jQuery);