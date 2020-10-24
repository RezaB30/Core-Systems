function SetupWebPhone() {
    if ($('div.call-center-box').length > 0) {
        $('div.call-center-box').click(function () {
            $('div.call-center-details-box').fadeToggle(200);
        });

        $('div.call-center-details-box').find('input#webphone-button').click(function () {
            window.open($(this).attr('link-ref'), "popupWindow", "width=280,height=610,toolbar=no,location=no,status=no,menubar=no,scrollbars=no,resizable=no,fullscreen=no");
        });

        var intervalRunning = false;

        setInterval(function () {
            if (intervalRunning)
                return;
            intervalRunning = true;
            $.ajax({
                method: 'POST',
                url: $('#call-center-events-container').attr('refresh-ref'),
                data: { lastUUID: $('#call-center-events-container').attr('last-uuid'), lastEventType: $('#call-center-events-container').attr('last-event-type') },
                complete: function (data, status) {
                    if (status == 'success') {
                        if (!$.isEmptyObject(data.responseJSON)) {
                            $('#call-center-events-container').attr('last-uuid', data.responseJSON.RawEvent.call_uuid);
                            $('#call-center-events-container').attr('last-event-type', data.responseJSON.RawEvent.event_type);
                            CreateEvent(data.responseJSON);
                            //alert(data.responseText);
                        }
                    }
                    //else {
                    //    $('#call-center-events-container').append(status);
                    //}
                    intervalRunning = false;
                }
            });
        }, 500);

        function CreateEvent(callCenterEvent) {
            var actionLink = "";
            if (callCenterEvent.Url != null)
                actionLink = "<a href='" + callCenterEvent.Url + "' class='link-button iconed-button next-button' target='_blank'>" + callCenterEvent.LinkText + "</a>";
            var now = new Date();
            var content = "<div class='event-message'><div class='event-time'>" + now.getHours().toString().padStart(2, "0") + ":" + now.getMinutes().toString().padStart(2, "0") + ":" + now.getSeconds().toString().padStart(2, "0") + "</div>" + callCenterEvent.Message + "<div>" + actionLink + "</div></div>";
            $('#call-center-events-container').prepend(content);
            if ($('#call-center-events-container').find('div.event-message').length > 1) {
                $('#call-center-events-container').find('div.event-message').last().remove();
            }
            $('div.call-center-details-box:hidden').fadeIn(200);
        }
    }
}

function CallCenterCall(phoneNo, url) {
    ShowConfirm(function () {
        $.ajax({
            url: url,
            data: { number: phoneNo },
            method: 'POST'
        });
    }, false);
}