function SetupSubscriptionAddFee(containerId) {
    var container = $(containerId);
    container.find('div.add-fee-editor-wrapper').each(function () {
        var currentWrapper = $(this);
        var addRow = currentWrapper.find('div.add-instance-row');
        var addButton = addRow.find("input.add-instance-button");
        var sampleContainer = currentWrapper.find('div.sample-container');
        var sampleMasterContainer = sampleContainer.find('ol.sample-master-container');
        var list = currentWrapper.children('ol.multiselect-orderedlist');

        SetupAddedItems(list.find('li'));

        addButton.click(function () {
            list.append(sampleMasterContainer.html());
            var addedItem = list.find('li').last();
            SetupAddedItems(addedItem);
        });

        SetupParentForm();

        function SetupAddedItems(addedItems) {
            $(addedItems).each(function () {
                var addedItem = $(this);

                SetupSelectLists(addedItem);

                addedItem.children('div.type-selection-container').children('input.remove-instance-button').click(function () {
                    $(this).closest('li.multiselect-input-row').remove();
                });

                var addedSelectList = addedItem.find('div.select-list-wrapper').find('input[type=hidden]');
                var addedItemSubContainer = addedItem.find('div.item-selection-results-container');
                addedSelectList.change(function () {
                    if (addedSelectList.val() != '') {
                        addedItemSubContainer.html(sampleContainer.find('div.sample-item[value="' + addedSelectList.val() + '"]').html());
                        SetupSelectLists(addedItemSubContainer);
                        SetupCustomFees(addedItemSubContainer);
                    }
                    else {
                        addedItemSubContainer.html('');
                    }
                });

                SetupCustomFees(addedItemSubContainer);
            });
        }

        function SetupCustomFees(customFeeContainerId) {
            var container = $(customFeeContainerId);
            container.find('div.add-fee-custom-fee-wrapper').each(function () {
                var currentWrapper = $(this);
                var addRow = currentWrapper.find('div.add-sub-instance-row');
                var addButton = addRow.find("input.add-instance-button");
                var sampleContainer = currentWrapper.find('ol.custom-fee-sample');
                var list = currentWrapper.children('ol.multiselect-orderedlist');

                addButton.click(function () {
                    list.append(sampleContainer.html());
                    var addeItem = list.find('li').last();
                    SetupSelectLists(addeItem);
                    
                    addeItem.find('input.remove-instance-button').click(function () {
                        $(this).closest('li.multiselect-input-row').remove();
                    });
                });

                list.find('li').find('input.remove-instance-button').click(function () {
                    $(this).closest('li.multiselect-input-row').remove();
                });
            });
        }

        function SetupParentForm() {
            var parentForm = currentWrapper.closest('form').submit(function (e) {
                sampleContainer.remove();
                currentWrapper.find('ol.custom-fee-sample').remove();
                currentWrapper.find('input.add-instance-button,input.remove-instance-button').remove();
                list.children('li.multiselect-input-row').each(function (masterIndex) {
                    var currentRow = $(this);
                    currentRow.find('input[name*=".sample."]').each(function () {
                        var currentInput = $(this);
                        currentInput.attr('name', currentInput.attr('name').replace(/(\.sample\.)/, '[' + masterIndex + '].'));
                    });
                    currentRow.find('div.add-fee-custom-fee-wrapper').children('ol.multiselect-orderedlist').children('li.multiselect-input-row').each(function (slaveIndex) {
                        var currentRow = $(this);
                        currentRow.find('input[name*=".sample."]').each(function () {
                            var currentInput = $(this);
                            currentInput.attr('name', currentInput.attr('name').replace(/(\.sample\.)/, '[' + slaveIndex + '].'));
                        });
                    });
                });
            });
        }
    });
}