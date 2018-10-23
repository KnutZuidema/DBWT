$(function () {
    let checkboxes = $(':checkbox');
    let cards = $('.card');
    checkboxes.click(function () {
        let checkbox = $(this);
        let selected = checkbox.attr('id');
        if(checkbox.is(':checked')){
            cards.each(function () {
                let card = $(this);
                if(!card.hasClass(selected)){
                    card.hide();
                }
            })
        }else{
            let checked = [];
            checkboxes.each(function () {
                let checkbox = $(this);
                if(checkbox.is(':checked') && checkbox.attr('id') !== selected){
                    checked.push(checkbox.attr('id'));
                }
            });
            if(!checked.length){
                cards.show();
                return
            }
            cards.each(function () {
                let card = $(this);
                card.hide();
                let class_list = card.attr('class').split(/\s+/);
                for(let check of checked){
                    for(let clazz of class_list){
                        if(check === clazz) {
                            card.show();
                        }
                    }
                }
            })
        }
    })
});