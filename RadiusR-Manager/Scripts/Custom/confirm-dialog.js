function ShowConfirm(callback, showTimer) {
    if (showTimer == null)
        showTimer = true;

    var sample = $('div.attachments').find('div.confirm-message');
    var timer = $('div.attachments').find('div.waiting-progress-fixed-wrapper');

    var lastElement = $('body').children().last();
    lastElement.after(sample.html());
    var currentDialog = lastElement.next();
    var noButton = currentDialog.find('input[name="No"]');
    var yesButton = currentDialog.find('input[name="Yes"]');

    noButton.click(function () {
        currentDialog.remove();
    });
    yesButton.click(function () {
        if (showTimer) {
            lastElement.after(timer.html());
            var currentTimer = lastElement.next();
            StartWaitingCounter(currentTimer);
            currentTimer.attr("id", "active-timer");
        }
        currentDialog.remove();
        callback();
    });
}