function fillPartTap() {
    var p = document.getElementById("partNum").value;
    var qty = document.getElementById("qty").value;
    $.ajax({
        method: "POST",
        url: "/Part/getPartByPN",
        dataType: "json",
        data: { partnum: p }
    }).done(function (response) {
        var partnum = response.code;
        $.ajax({
            method: "POST",
            url: "/Part/InsertToPTL",
            dataType: "json",
            data: { p: response, qty: qty }
        }).done(function(){
            fillPart(partnum, qty);
        })      
    })
}

function fillPart(part, qty) {
    $.ajax({
        method: "POST",
        url: "/Part/fillPart",
        dataType: "json",
        data: { p: part, quantity: qty}
    }).done(function (response) {
        if (response = true) {
            document.getElementById("snlabel").innerText = "Save succeed."
        } else {
            document.getElementById("snlabel").innerText = ":("
        }
    })
}