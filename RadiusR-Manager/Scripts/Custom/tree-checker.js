function SetupTreeChecker(containerId) {
    var container = $(containerId);
    var trees = container.find('.tree-checker-wrapper');
    trees.each(function () {
        var currentWrapper = $(this);
        var hiddenInput = currentWrapper.find('input[type=hidden]');
        var currentForm = currentWrapper.closest('form');
        currentForm.submit(function () {
            var lists = currentWrapper.find('ul.tree-checker-list.selected:not(.tree-checker-wrapper)').map(function () { return $(this).attr('tree-node-value') });
            hiddenInput.val(lists.toArray().join(','));
        });
        var titles = currentWrapper.find('div.tree-checker-title');
        titles.each(function () {
            var currentTitle = $(this);
            currentTitle.click(function (e, programatic) {
                var currentList = currentTitle.closest('ul.tree-checker-list');
                var parentList = currentList.parent().closest('ul.tree-checker-list');//:not(.tree-checker-wrapper)
                //alert(programatic);
                if (parentList.length > 0 && !parentList.hasClass('tree-checker-wrapper') && !parentList.hasClass('selected') && !currentList.hasClass('selected')) {
                    return;
                }
                if (parentList.length <= 0) {
                    currentList.toggleClass('selected');
                }
                else {
                    if (parentList.hasClass('selected') || parentList.hasClass('tree-checker-wrapper')) {
                        if (parentList.hasClass('tree-checker-wrapper') && parentList.hasClass('selected')) {
                            if (currentList.hasClass('selected') && typeof programatic != 'undefined')
                                return;
                            currentList.toggleClass('selected');
                        }
                        else {
                            if (parentList.hasClass('tree-checker-wrapper') && !parentList.hasClass('selected') && !currentList.hasClass('selected') && typeof programatic != 'undefined')
                                return;
                            currentList.toggleClass('selected');
                        }

                    }
                    else {
                        if (!currentList.hasClass('selected'))
                            return;
                        currentList.removeClass('selected');
                    }
                }
                currentTitle.siblings('ul').children('li').children('div.tree-checker-title').trigger('click', ['programatic']);
            });
        });
    });
}