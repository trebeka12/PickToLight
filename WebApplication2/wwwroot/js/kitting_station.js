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
                let product = response;
                console.log(product)
                kittingSN(product);
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
        data: { p: p }
    }).done(function (response) {
        if (response) {
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
            }).done(function (response) {
                document.getElementById("kittinglabel").innerText = "BOM list loaded." 
            })       
        } else {
            document.getElementById("kittinglabel").innerText = "Something went wrong..."
        }
   })
}