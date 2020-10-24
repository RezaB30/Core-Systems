function SetupTextListEditors(containerId) {
    var workArea = $(containerId);
    workArea.find(".text-list-editor-container").each(function () {
        var wrapper = $(this);
        var addRowButton = wrapper.find(".add-instance-button").first();
        var sample = wrapper.find(".text-list-editor-sample").first();

        var orderedListHtml = "<ol class='multiselect-orderedlist'></ol>";
        addRowButton.before(orderedListHtml);
        var orderedlist = addRowButton.prev(".multiselect-orderedlist");
        addRowButton.click(function () {
            addInstance(orderedlist, sample);
        });

        //add pre-selected instances
        if (sample.attr("selected_values") != "") {
            var selectedValues = sample.attr("selected_values").split(",");
            for (var i = 0; i < selectedValues.length; ++i) {
                addInstance(orderedlist, sample, selectedValues[i]);
            }
        }

        //prepare for post
        wrapper.closest('form').submit(function () {
            PrepareToPost(wrapper);
        });

        wrapper.find(".remove-instance-button").click(function () {
            $(this).parents("li").slideUp(200, function () { $(this).remove(); });
        });
    });

    function addInstance(orderedlist, sample, selectedValue) {
        orderedlist = $(orderedlist);
        orderedlist.append("<li class='multiselect-input-row'></li>");
        var newRow = orderedlist.find("li").last();
        newRow.hide();
        newRow.html(sample.html());

        var removeButton = newRow.find(".remove-instance-button");
        removeButton.click(function () {
            $(this).parents("li").slideUp(200, function () { $(this).remove(); });
        });

        if (typeof selectedValue != "undefined") {
            newRow.find("input[type=text]").val(selectedValue);
        }

        newRow.slideDown(200);
    }

    function PrepareToPost(containerId) {
        var workArea = $(containerId);
        var list = workArea.find("ol.multiselect-orderedlist").first();
        var instances = list.find("li");
        instances.each(function () {
            var current = $(this);
            var currentInput = current.find("input[type=text]");
            if (currentInput.val() == '') {
                current.html('');
                current.hide();
            }
        });
        workArea.find(".text-list-editor-sample").remove();
    }
}