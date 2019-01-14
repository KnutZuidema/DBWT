$(function () {
    $('#order_button').click(function () {
        let orders = Cookies.get('orders');
        if (orders === undefined) orders = '{}';
        orders = JSON.parse(orders);
        if (!orders.hasOwnProperty('Meals')) orders.Meals = {};
        if (!orders.hasOwnProperty('PickupTime')) orders.PickupTime = null;
        let id = window.location.pathname.split('/').pop();
        let amount = orders.Meals[id];
        amount = amount === undefined ? 1 : amount + 1;
        orders.Meals[id] = amount;
        Cookies.set('orders', JSON.stringify(orders), { expires: 1 });
    })
});