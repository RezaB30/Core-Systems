function SetupFileUploads(containerId) {
    var container = $("body");
    if (typeof containerId != "undefined")
        container = $(containerId);

    container.find(".file-upload-wrapper").each(function () {
        var currentWrapper = $(this);
        var hiddenFile = currentWrapper.find(".hidden-file-upload").first();
        var browseButton = currentWrapper.find(".upload-file-browse").first();
        var fileText = currentWrapper.find(".file-upload-text").first();
        browseButton.click(function () {
            hiddenFile.click();
        });
        hiddenFile.change(function () {
            var rawValue = hiddenFile.val() + "";
            fileText.val(rawValue.substring(rawValue.lastIndexOf('\\') + 1, rawValue.length));
        });
    });
}