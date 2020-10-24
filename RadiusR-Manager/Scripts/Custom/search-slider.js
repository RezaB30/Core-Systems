$(document).ready(function () {
    $('.slider-lip').click(function () {
        toggleSearchSlider();
    });

    $('div.slider-contents').find('input').keydown(function (e) {
        var current = $(this);
        if (e.keyCode == 192 || e.keyCode == 162) {
            e.preventDefault();
        }
    });

    $(document).keyup(function (e) {
        if (e.keyCode == 192 || e.keyCode == 162) {
            toggleSearchSlider();
        }
    });
});

function toggleSearchSlider() {
    $('div.slider-contents').toggleClass('opened');
    if ($('div.slider-contents').hasClass('opened')) {
        $('div.slider-contents').find('input').first().focus();
    }
}