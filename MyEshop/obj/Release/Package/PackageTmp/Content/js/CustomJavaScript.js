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
    });
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

function AddToCompare(id)
{
    $.get("/Compare/AddToCompare/" + id, function (res) {
        $("#Compare").html(res);
    });
}

function DeleteFromCompare(id) {
    $.get("/Compare/DeleteFromCompare/" + id, function (res) {
        $("#Compare").html(res);
        $("#chech_"+id).attr('checked', false);
    })
}