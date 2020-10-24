///<reference path="Scripts/jquery-{version}.js" />
///<reference path="Scripts/jquery-{version}.intellisense.js" />

function SetupCheckbuttons(containerId) {
    var containers =$(containerId).find(".checkbutton-container");
    containers.each(function () {
        var currentContainer = $(this);
        var button = currentContainer.find(".check-button").first();
        var checkbox = currentContainer.find("input[type=checkbox]").first();
        button.click(function () {
            button.toggleClass("selected");
            if (button.hasClass("selected")) {
                checkbox.prop("checked", true);
            }
            else {
                checkbox.prop("checked", false);
            }
            checkbox.trigger("change");
        });
    });
}

