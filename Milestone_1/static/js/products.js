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
            cards.show();
            if(!checked.length){
                return
            }
            cards.each(function () {
                let card = $(this);
                let class_list = card.attr('class').split(/\s+/);
                for(let check of checked){
                    if(!class_list.includes(check)){
                        card.hide();
                        break
                    }
                }
            })
        }
    })
});