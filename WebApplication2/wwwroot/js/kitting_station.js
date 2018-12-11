async function kittingSNTap() {
    document.getElementById("kittinglabel").innerText = "loading..." 
    let selectedSerialNumber = document.getElementById("serialNumber").value
    if (selectedSerialNumber != null & selectedSerialNumber.length > 0) {            
        console.log(selectedSerialNumber)
            $.ajax({
                method: "POST",
                url: "/Product/getProductBySN",
                dataType: "json",
                data: { serialnum: selectedSerialNumber }
            }).done(function (response) {
                if (response.stationID === 1) {
                    console.log(response)
                    kittingSN(response);
                } else if (response.stationID === 2){
                    alert("Go to the Assembly station!")
                    document.getElementById("kittinglabel").innerText = ""

                }
                else if (response.stationID === 3) {
                    alert("Go to the Pack station!")
                    document.getElementById("kittinglabel").innerText = ""

                }
                else if (response.stationID === 4) {
                    alert("You are done!")
                    document.getElementById("kittinglabel").innerText = ""
s
                }
            })
    } else {
        console.log(error)
    }
}

async function kittingSN(product) {
    let p = product;
    console.log(p)
    $.ajax({
        method: "POST",
        url: "/Kitting/GetBoms",      
        dataType: "json",
zalrtars    }).done(function (response) {
        if (response){
            $.each(response, function (k, v) {
                $("#bomlist").append($('<li>', {
                    value: v["id"],
                    text: v["partName"]
                }));
            });
            $.ajax({
                method: "POST",
                url: "/Kitting/InsertToPTL",
                dataType: "json",
                data: { p: response }
            }).done(function () {
                document.getElementById("kittinglabel").innerText = "BOM list loaded."
                })
        }
   })
}