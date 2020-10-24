function initializeConfirm(containerId, loader) {
    var sample = $('div.attachments').find('div.confirm-message');
    $(containerId).find('form[confirm="enabled"]').each(function () {
        var currentForm = $(this);
        currentForm.submit(function (e, data) {
            if (data == null)
            {
                e.preventDefault();
                ShowConfirm(function () {
                    if (loader != null) {
                        loader.Load(currentForm.attr('action'), currentForm.serialize(), currentForm.attr("method"));
                        currentDialog.remove();
                    }
                    else
                        currentForm.trigger('submit', true);
                });
            }
        });
    });
}