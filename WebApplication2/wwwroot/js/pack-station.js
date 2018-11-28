async function packSNTap() {
    document.getElementById("packlabel").innerText = "loading..." 
    let selectedSerialNum = document.getElementById("packSerialNumber").value
    console.log(selectedSerialNum)
    if (selectedSerialNum != null & selectedSerialNum.length > 0) {
        console.log(selectedSerialNum)
        $.ajax({
            method: "POST",
            url: "/Product/getProductBySN",
            dataType: "json",
            data: { serialnum: selectedSerialNum }
        }).done(function (response) {
            console.log(response)
            if (response.isComplete === true) {
                document.getElementById("packlabel").innerText = "Pack is complete yet! ( SN + " + response.serialNumber + ")";
            } else {
                packSN(response);
            }
        })
    } else {
        console.log("error")
       document.getElementById("packlabel").innerText = "Invalid SerialNumber ( " + document.getElementById("packSerialNumber").value + "), pack is uncomplete.";
    }
}

async function packSN(product) {
    let s = product.serialNumber;
    $.ajax({
        method: "POST",
        url: "/Product/packSN",
        dataType: "json",
        data: { s: s }
    }).done(function (response) {
        document.getElementById("packlabel").innerText = "Pack is completed. ( SN + " + response.serialNumber + ")";
    })
}