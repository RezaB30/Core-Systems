$(document).ready(function () {
    $('.card-payment-wrapper').each(function () {
        var currentWrapper = $(this);
        var currentForm = currentWrapper.closest('form');

        currentWrapper.find('input[type="text"]:nth-of-type(1),input[type="text"]:nth-of-type(4)').keydown(function (e) {
            if (e.shiftKey || e.altKey || e.ctrlKey) {
                e.preventDefault();
            }
            if ((e.which >= 48 && e.which <= 57) || (e.which >= 96 && e.which <= 105)) {

            }
            else if (e.which != 9 && e.which != 8 && e.which != 46 && !(e.which >= 37 && e.which <= 40)) {
                e.preventDefault();
            }
        });

        currentWrapper.find('input[type="text"]:nth-of-type(1)').bind('input', function () {
            var value = $(this).val().substring(0, 19);
            value = value.replace(/\ /g, '');
            var length = value.length;
            for (var i = 0; i < value.length; i++) {
                if ((i != 0) && ((i % 4) - Math.floor(i / 4) + 1 == 0)) {
                    if (value[i] != '-')
                        value = [value.slice(0, i), ' ', value.slice(i)].join('');
                    length++;
                }
            }

            $(this).val(value);
        });

        currentWrapper.find('input[type="text"]:nth-of-type(1)').trigger('input');

        currentForm.submit(function () {
            var cardNoInput = currentWrapper.find('input[type="text"]:nth-of-type(1)');
            cardNoInput.val(cardNoInput.val().replace(/\ /g, ''));
        });
    });
});