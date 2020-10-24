function SetupSliderBoxes(containerId) {
    var container = $(containerId);
    var sliderButtons = container.find('.sliding-box-button');
    var slides = container.find('.sliding-box-slide');

    sliderButtons.each(function (index) {
        $(this).click(function () {
            var currentButton = $(this);
            var currentSlide = $(slides.get(index));
            slides.removeClass('left');
            slides.removeClass('right');
            currentSlide.prevAll().addClass('left');
            currentSlide.nextAll().addClass('right');
            currentSlide.removeClass('left');
            currentSlide.removeClass('right');
            sliderButtons.removeClass('selected');
            currentButton.addClass('selected');
        });
    });
}