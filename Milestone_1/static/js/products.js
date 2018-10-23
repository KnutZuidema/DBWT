$(function () {
    let checkboxes = $(':checkbox');
    let cards = $('.card');
    $('[type="checkbox"]:checked').each(function () {
        hide_without_class(cards, $(this).attr('id'));
    });
    checkboxes.click(function () {
        cards.show();
        let checkbox = $(this);
        let selected = checkbox.attr('id');
        let checked = get_other_checked(checkboxes, selected);
        if (checkbox.is(':checked')) {
            checked.push(selected);
        } else {
            if (!checked.length) {
                return
            }
        }
        checked.forEach(check => hide_without_class(cards, check))
    })
});

function hide_without_class(elements, clazz) {
    elements.each(function () {
        let element = $(this);
        if (!element.hasClass(clazz)) {
            element.hide();
        }
    })
}

function get_other_checked(checkboxes, this_value) {
    let checked = [];
    checkboxes.each(function () {
        let checkbox = $(this);
        if (checkbox.is(':checked') && checkbox.attr('id') !== this_value) {
            checked.push(checkbox.attr('id'));
        }
    });
    return checked
}