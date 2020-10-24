function SetupPhoneNoEditors(containerId) {
    var container = $(containerId);
    var editors = container.find('input.input-phone');
    editors.each(function () {
        var currentEditor = $(this);
        currentEditor.bind('input', function () {
            var currentValue = currentEditor.val();
            if (currentValue == '+')
                return;
            var parts = currentValue.split(' ');
            for (var i = 0; i < parts.length; i++) {
                parts[i] = parts[i].replace(/[\D\s]/g, '');
            }
            parts = parts.slice(0, 4);
            if (parts.length > 0) 
                parts[0] = parts[0].substring(0, 3);
            if (parts.length > 1) {
                if (parts[1].length > 3) {
                    if (parts.length > 2)
                        parts[2] = parts[1].substring(3) + parts[2];
                    else
                        parts[2] = parts[1].substring(3);
                }
                parts[1] = parts[1].substring(0, 3);
            }
            if (parts.length > 2) {
                if (parts[2].length > 3) {
                    if (parts.length > 3)
                        parts[3] = parts[2].substring(3) + parts[3];
                    else
                        parts[3] = parts[2].substring(3);
                }
                parts[2] = parts[2].substring(0, 3);
            }
            if (parts.length > 3)
                parts[3] = parts[3].substring(0, 4);
            currentValue = parts.join(' ');
            if (currentValue != '')
                currentValue = '+' + currentValue;
            currentEditor.val(currentValue);
        });
    });
}