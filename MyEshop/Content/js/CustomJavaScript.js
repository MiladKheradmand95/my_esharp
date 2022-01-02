$(function () {
    countShopCart();
});
function countShopCart() {
    $.get("/Api/Shop", function (res) {
        $("#countShopCart").html(res);
    });
}

function ShopCart(id) {
    $.get("/Api/Shop/" + id, function (res) {
        $("#countShopCart").html(res);
        UpdateShopCart();
    })
}


function UpdateShopCart() {
    $("#ShowCart").load("/ShopCart/ShowCart").fadeOut(800).fadeIn(800);
}



function commandOrder(id, count) {
    $.ajax({
        url: "/ShopCart/commandOrder/" + id,
        type: "Get",
        data: { count: count }
    }).done(function (res) {
        $("#ShowOrder").html(res);
        UpdateShopCart();
        countShopCart();
    });
}