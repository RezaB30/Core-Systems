function
    GetJson(loadingUrl, parameters, method, callback) {
    var results = null;
    $.ajax(loadingUrl, {
        context: this,
        data: parameters,
        method: method,
        complete: function (data, status) {
            if (status == "success") {
                callback(data.responseJSON);
            }
            else {
                callback(null);
            }
        }
    });
}