$(function () {
    let checkboxes = $('[type=checkbox]');
    let cards = $('.card');
    let select = $("select");
    filter(cards);
    checkboxes.click(() => filter(cards));
    select.change(() => filter(cards))
});

function filter(elements) {
    elements.show();

    $('[type="checkbox"]:checked').each(function () {
        hide_without_class(elements, $(this).attr('id'));
    });
    let value = $('select').val();
    if(value !== '0'){
        hide_without_class(elements, `category-${value}`);
    }
}

function hide_without_class(elements, clazz) {
    elements.each(function () {
        let element = $(this);
        if (!element.hasClass(clazz)) {
            element.hide();
        }
    })
}