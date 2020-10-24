function SetupCreditForms(containerId) {
    var container = $(containerId);
    var addButton = container.find("#add-button").first();
    var subButton = container.find("#subtract-button").first();
    var addForm = container.find("#add-form").first();
    var subForm = container.find("#subtract-form").first();
    var cancelButtons = container.find("#add-form,#subtract-form").find(".cancel-button");

    addButton.click(function () {
        addButton.hide();
        subButton.hide();
        addForm.fadeIn(200);
    });

    subButton.click(function () {
        addButton.hide();
        subButton.hide();
        subForm.fadeIn(200);
    });

    cancelButtons.click(function () {
        var button = $(this);
        button.closest('.edit-credit-form-container').hide();
        addButton.fadeIn(200);
        subButton.fadeIn(200);
    });
}