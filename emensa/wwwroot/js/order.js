$(function () {
    $(".amount").change(function () {
        let cookie = JSON.parse(Cookies.get('orders'));
        let amount = $(this).val();
        let parent = $(this).parent().parent();
        let id = parent.find('.id').text();
        cookie.Meals[id] = amount;
        Cookies.set('orders', JSON.stringify(cookie), {expires: 1});
        let price = parseFloat(parent.find(".single-price").text());
        let totalPrice = parent.find(".total-price");
        let difference = amount * price - parseFloat(totalPrice.text());
        totalPrice.text((amount * price).toFixed(2));
        let totalSum = $("#total-price");
        totalSum.text((parseFloat(totalSum.text()) + difference).toFixed(2));
    });

    $("#pickup-time").change(function () {
        let cookie = JSON.parse(Cookies.get('orders'));
        cookie.PickupTime = $(this).val();
        Cookies.set("orders", JSON.stringify(cookie), { expires: 1 });
    });

    $('#delete-all').click(function () {
        Cookies.remove('orders');
        $('.meal').remove();
    })
});