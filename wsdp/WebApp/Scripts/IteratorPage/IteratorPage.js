﻿$(function () {
    $(".list-inline-item:nth-child(2)").addClass("active");

    enableXpathSearch();

    $('#var2').click(function () {
        enableUrlSearch();
    });

    $('#var1').click(function () {
        enableXpathSearch();
    });
});

function enableUrlSearch() {
    $("#XPathPageIterator").attr("disabled", "disabled");

    $("#UrlMask").removeAttr("disabled");
    $("#From").removeAttr("disabled");
    $("#To").removeAttr("disabled");
};
function enableXpathSearch() {
    $("#XPathPageIterator").removeAttr("disabled");

    $("#UrlMask").attr("disabled", "disabled");
    $("#From").attr("disabled", "disabled");
    $("#To").attr("disabled", "disabled");

};