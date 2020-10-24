function SetupIPLoader(nasListContainerId, SharedIPListContainerId, IPListRetrievingURL, sharedIPInitialValue, StaticIPListContainerId, staticIPInitialValue, clientId) {

    var nasListInput = $(nasListContainerId).find("input[name]").first();
    var SharedIPListInput = $(SharedIPListContainerId).find("input[name]").first();
    var SharedIPListOptionsList = $(SharedIPListContainerId).find("div.options-container").first();
    var StaticIPListInput = $(StaticIPListContainerId).find("input[name]").first();
    var StaticIPListOptionsList = $(StaticIPListContainerId).find("div.options-container").first();
    var sharedIPCells = $('.shared-ip-cell');
    var staticIPCells = $('.static-ip-cell');
    var IPTypeHead = $('.ip-type-head');
    var naturalIPText = IPTypeHead.attr('ip-value-natural');
    var sharedIPText = IPTypeHead.attr('ip-value-shared');
    var staticIPText = IPTypeHead.attr('ip-value-static');

    function FillSharedIPList(value) {
        // clear the list
        selectOption("", $(SharedIPListContainerId).find('div.select-list-wrapper'));
        var currentContainer = SharedIPListOptionsList.children().first();
        currentContainer.children(':not([value=""])').remove();

        if (nasListInput.val()) {
            // retrieve IPs
            $.ajax(IPListRetrievingURL, {
                context: SharedIPListOptionsList,
                data: { NasId: nasListInput.val(), ClientID: clientId },
                method: "POST",
                complete: function (data, status) {
                    if (status == "success") {
                        var retrievedIPs = data.responseJSON;
                        var addedContent = "";
                        for (var i = 0; i < retrievedIPs.length; i++) {
                            addedContent = addedContent.concat("<div class='list-option' value='" + retrievedIPs[i] + "'>" + retrievedIPs[i] + "</div>");
                        }
                        currentContainer.html(currentContainer.html() + addedContent);
                        if (value == null)
                            value = "";
                        currentContainer.children('[value="' + value + '"]').addClass('selected');
                        SharedIPListInput.val(value);
                        SetEvents();
                        SharedIPListInput.trigger('change');
                    }
                }
            });
        }
    }

    function FillStaticIPList(value) {
        // clear the list
        selectOption("", $(StaticIPListContainerId).find('div.select-list-wrapper'));
        var currentContainer = StaticIPListOptionsList.children().first();
        currentContainer.children(':not([value=""])').remove();

        if (nasListInput.val()) {
            // retrieve IPs
            $.ajax(IPListRetrievingURL, {
                context: StaticIPListOptionsList,
                data: { NasId: nasListInput.val(), IsStatic: 'on', ClientID: clientId },
                method: "POST",
                complete: function (data, status) {
                    if (status == "success") {
                        var retrievedIPs = data.responseJSON;
                        var addedContent = "";
                        for (var i = 0; i < retrievedIPs.length; i++) {
                            addedContent = addedContent.concat("<div class='list-option' value='" + retrievedIPs[i] + "'>" + retrievedIPs[i] + "</div>");
                        }
                        currentContainer.html(currentContainer.html() + addedContent);
                        if (value == null)
                            value = "";
                        currentContainer.children('[value="' + value + '"]').addClass('selected');
                        StaticIPListInput.val(value);
                        SetEvents();
                        StaticIPListInput.trigger('change');
                    }
                }
            });
        }
    }

    function SetEvents() {
        $(SharedIPListContainerId).find('*').off();
        SetupSelectLists(SharedIPListContainerId);
        $(StaticIPListContainerId).find('*').off();
        SetupSelectLists(StaticIPListContainerId);
        // set visuals
        SharedIPListInput.change(function () {
            if (SharedIPListInput.val() == '') {
                IPTypeHead.removeClass('shared');
                staticIPCells.removeClass('ignore');
                if (StaticIPListInput.val() == '')
                    IPTypeHead.text(naturalIPText);
            }
            else {
                selectOption('', $(StaticIPListContainerId).find('div.select-list-wrapper'));
                IPTypeHead.removeClass('static');
                IPTypeHead.addClass('shared');
                IPTypeHead.text(sharedIPText);
                staticIPCells.addClass('ignore');
            }
        });

        StaticIPListInput.change(function () {
            if (StaticIPListInput.val() == '') {
                IPTypeHead.removeClass('static');
                sharedIPCells.removeClass('ignore');
                if (SharedIPListInput.val() == '')
                    IPTypeHead.text(naturalIPText);
            }
            else {
                selectOption('', $(SharedIPListContainerId).find('div.select-list-wrapper'));
                IPTypeHead.removeClass('shared');
                IPTypeHead.addClass('static');
                IPTypeHead.text(staticIPText);
                sharedIPCells.addClass('ignore');
            }
        });
    }

    nasListInput.change(function () {
        FillStaticIPList('');
        FillSharedIPList('');
    });

    FillSharedIPList(sharedIPInitialValue);
    FillStaticIPList(staticIPInitialValue);
}