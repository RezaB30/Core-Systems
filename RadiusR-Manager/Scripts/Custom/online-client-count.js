function SetOnlineCount() {
    var container = $("#online-count-container");
    if (container.length > 0) {
        $.ajax({
            url: container.attr('load-ref'),
            method: 'POST',
            complete: function (data, status) {
                if (status == 'success') {
                    container.find('div.online-count').text(data.responseText);
                }
                else {
                    container.find('div.online-count').text('-!!!-');
                }
            }
        })
    }
}