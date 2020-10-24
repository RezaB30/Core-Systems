function SetupAddressSearch(containerId) {
    var allWrappers = $(containerId).find('div.address-search-wrapper');
    allWrappers.each(function () {
        var currentWrapper = $(this);
        var selectButton = currentWrapper.find('input.open-list-button');
        var addressText = currentWrapper.find('div.address-text');
        var editorWrapper = currentWrapper.find('div.address-input-wrapper');
        var okButton = editorWrapper.find('input.close-list-button');

        okButton.click(function () {
            CloseEditor();
        });
        $(document).click(function () {
            CloseEditor();
        });
        editorWrapper.click(function (e) {
            e.stopPropagation();
        });

        function CloseEditor() {
            if (editorWrapper.is(':visible')) {
                editorWrapper.hide();
                SetText();
            }
        }

        selectButton.click(function (e) {
            e.stopPropagation();
            editorWrapper.fadeIn(200);
        });

        function SetText() {
            if (editorWrapper.find('span.address-text-display').text().length > 1) {
                addressText.text(editorWrapper.find('span.address-text-display').text());
            }
            else {
                var finalText = "";
                editorWrapper.find('input[type=text]').each(function () {
                    if ($(this).closest('div.select-list-wrapper').find('input[type=hidden]').val() != "")
                        finalText += $(this).val() + " ";
                });

                addressText.text(finalText);
            }
        }

        SetText();
    });
}