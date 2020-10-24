function LoadClientConnectionStatus(usernameSourceInputId, groupWrapperId, loaderActionLink, onlineTitle, offlineTitle, errorTitle, errorMessage) {
    var sources = $(usernameSourceInputId);

    $.ajax(loaderActionLink, {
        method: "POST",
        complete: function (data, status) {
            if (status == "success") {
                var response = data.responseJSON;
                sources.each(function () {
                    var currentInput = $(this);
                    var currentUsername = currentInput.val();
                    var statusContainer = currentInput.closest(groupWrapperId).find('.online-status-container');
                    var isOnline = false;
                    response.forEach(function (item) {
                        if (item != null && item.Users != null && item.Users.indexOf(currentUsername) > -1)
                            isOnline = true;
                    });
                    if (isOnline) {
                        statusContainer.html('<div class="online-status-online" title="' + onlineTitle + '"></div>');
                    }
                    else {
                        statusContainer.html('<div class="online-status-offline" title="' + offlineTitle + '"></div>');
                    }
                });
                // show error if needed
                var downedNasNames = [];
                response.forEach(function (item) {
                    if (item != null && item.Users == null) {
                        downedNasNames.push(item.NASIP);
                    }
                });
                if (downedNasNames.length > 0) {
                    showDownNASError(downedNasNames.join('<br />'));
                }
            }
            else {
                $('td.online-status-container').html('<div class="online-status-error" title="' + errorTitle + '"></div>');
            }
        }
    });

    var showDownNASError = function (NASNames) {
        var pageContainer = $("#page-contents");
        pageContainer.prepend("<div class='client-state-alert-box'>" + errorMessage + "<br />" + NASNames + "<input type='button' /></div>");
        pageContainer.find('div.client-state-alert-box').find('input').click(function () {
            $(this).closest('div.client-state-alert-box').fadeOut();
        });
    }
}