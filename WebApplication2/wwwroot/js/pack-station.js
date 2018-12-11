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
            if (response.stationID === 1) {
                alert("Go to the Kitting station!")
                document.getElementById("packlabel").innerText = "" 
            } else if (response.stationID === 2) {
                alert("Go to the Assembly station!")
                document.getElementById("packlabel").innerText = "" 
            }
            if (response.stationID === 3) {
                console.log(response)
                packSN(response);
            } 
            else if (response.stationID === 4) {
                document.getElementById("packlabel").innerText = "Pack is complete yet! ( SN + " + response.serialNumber + ")"
                document.getElementById("homeButton").disabled = false
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
        document.getElementById("homeButton").disabled = false
    })
}