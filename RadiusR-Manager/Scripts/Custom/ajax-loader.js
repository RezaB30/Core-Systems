function AjaxLoader(containerId, confineLinks) {
    var confine = false;
    if (confineLinks != null && confineLinks != false)
        confine = true;
    var loader = this;
    var container = $(containerId);
    var Callback = null;

    this.SetCallback = function(callback){
        Callback = callback;
    };

    this.Load = function (loadingLink, parameters, method) {
        container.html(GetLoadingPins());
        if (method == null)
            method = "GET";
        $.ajax(loadingLink, {
            context: container,
            data: parameters,
            method: method,
            complete: function (data, status) {
                container.html(data.responseText);
                if (status == "success") {
                    SetupLinearDiagrams(containerId);
                    setupDatepickers(containerId);
                    setupForms();
                    initializeConfirm(containerId, loader);
                    setupTablePages();
                    setupDailyMonthlyLink();
                    initializePartial(containerId);
                    if (confine) {
                        setupLinks();
                    }

                    if (Callback != null) {
                        Callback();
                    }
                }
                $('#active-timer').remove();
            }
        })
    };

    function setupForms() {
        var forms = container.find('form');
        forms.each(function () {
            var current = $(this);
            if (current.attr('confirm') != 'enabled') {
                current.submit(function (event) {
                    event.preventDefault();
                    loader.Load(current.attr('action'), current.serialize(), current.attr("method"));
                });
            }
        });
    }

    function setupTablePages() {
        var tablePageLinks = container.find('a.page-link:not(.disabled)').click(function (e) {
            e.preventDefault();
            var current = $(this);
            loader.Load(current.attr('href'));
        });
    }

    function setupDailyMonthlyLink() {
        var dailyMonthlyLink = container.find('a.calendar-button').click(function (e) {
            e.preventDefault();
            var current = $(this);
            loader.Load(current.attr('href'));
        })
    }

    function setupLinks() {
        var allLinks = container.find('a:not(.calendar-button):not(.page-link)').click(function (e) {
            e.preventDefault();
            var current = $(this);
            loader.Load(current.attr('href'));
        });
    }
}

function GetLoadingPins() {
    return "<div class='loading-pins-container'><div class='loading-pin pin1'></div><div class='loading-pin pin2'></div><div class='loading-pin pin3'></div></div>";
}