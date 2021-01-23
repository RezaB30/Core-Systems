///<reference path="../jquery/jquery-1.10.2.js" />
///<reference path="../jquery/jquery-1.10.2.intellisense.js" />
///<reference path="selectlist.js" />

function SetupMultiselectList(containerId) {
    ///<summary>Enables interactions with custom multiselect list.</summary>
    /// <param name="containerId" type="String">Selection list container id</param>

    var workArea = $(containerId);
    workArea.find(".multiselect-container").each(function () {
        var wrapper = $(this);
        var addRowButton = wrapper.find(".add-instance-button").first();
        var sample = wrapper.find(".multiselect-sample").first();

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
            wrapper.find('input[type=button]').remove();
            PrepareToPost(wrapper);
        });
    });

    function PrepareToPost(containerId) {
        ///<summary>Prepares a multiselect element for posting data by removing empty entries and serializing input names</summary>
        /// <param name="containerId" type="String">Selection list container id</param>

        var workArea = $(containerId);
        var list = workArea.find("ol.multiselect-orderedlist").first();
        var instances = list.find("li");

        var i = 0;
        instances.each(function () {
            var currentInstance = $(this);
            var currentHiddenInput = currentInstance.find("input[type=hidden]");
            if (currentHiddenInput.val() == "") {
                currentInstance.html("");
                currentInstance.hide();
            }
            else {
                currentHiddenInput.attr("name", currentHiddenInput.attr("name") + "[" + i + "]");
                i++;
            }
        });
        var instancesToRemove = list.find("li:hidden");
        instancesToRemove.remove();

        workArea.find(".multiselect-sample").remove();
    }

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

        newRow.slideDown(200);

        if (typeof selectedValue != "undefined") {
            newRow.find("input[type=hidden]").val(selectedValue);
        }

        SetupSelectLists(newRow);
    }
}



