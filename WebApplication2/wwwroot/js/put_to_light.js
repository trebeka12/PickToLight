function fillPartTap() {
    var p = document.getElementById("partNum").value;
    $.ajax({
        method: "POST",
        url: "/Part/getPartByPN",
        dataType: "json",
        data: { partnum: p }
    }).done(function (response) {
        console.log(response)
        
        fillPart(response.code, document.getElementById("qty").value);
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