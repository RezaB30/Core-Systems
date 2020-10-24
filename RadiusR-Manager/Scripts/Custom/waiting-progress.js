function StartWaitingCounter(containerId) {
    var container = $(containerId);
    var counterContainer = container.find('div.waiting-progress-timer');
    var currentIteration = 0;
    container.fadeIn(200);

    window.setInterval(update, 1000);

    function update() {
        counterContainer.text(currentIteration);
        currentIteration++;
    }
}