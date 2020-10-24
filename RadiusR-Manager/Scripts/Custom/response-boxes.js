function initializeResponseBoxes() {
    var search = window.location.search.replace('?', '');
    if (search.includes('errorMessage=')) {
        var sample = $('div.attachments').find('div.remove-results-message');
        var lastElement = $('body').children().last();
        lastElement.after(sample.html());
        var currentDialog = lastElement.next();
        var okButton = currentDialog.find('input[name="Ok"]');

        okButton.click(function () {
            currentDialog.remove();
        });
    }
}