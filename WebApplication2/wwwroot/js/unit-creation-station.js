
window.onload = loadProduct;



async function loadProduct() {

        $.ajax({
            method: "POST",
            url: "/Part/GetProducts",
            dataType: "json"
        }).done(function (response) {
            //clear some input boxes
            if (response) {
                $.each(response, function (k, v) {
                    $("#productSelector").append($('<option>', {
                        value: v["id"], 
                        text: v["partName"]
                    }));
                });
                console.log(response)
                document.getElementById("createSNTap").disabled = false;
            } else {
                console.log("error")
            }

        })

}



async function createSN() {
    document.getElementById("responselabel").innerText = "loading..."
    try {
        let selectedItem = $("#productSelector option:selected").val();
        if (selectedItem != null) {
            $.ajax({
                method: "POST",
                url: "/Part/createSN",
                dataType: "json",
                data: { param: selectedItem }
            }).done(function (response) {
                console.log(response)
                var serialNumber = response.serialNumber;
                var partName = $("#productSelector option:selected").text();
                console.log(partName, serialNumber)
                    document.getElementById("responselabel").innerText = "Generated SerialNumber: " + response.serialNumber;
                    print(serialNumber, partName);
            })
        } else {
            document.getElementById("responselabel").innerText = "SerialNumber is not generated. "
        }
    } catch (error) {
        return null;
    }
}

async function print(serial, part) {
    $.ajax({
        method: "POST",
        url: "/Label/Print",
        dataType: "text",
        data: {serial: serial, part:part}
    }).done(function (response) {
        console.log(response)
        localpost(response);
    })
}

async function localpost(label) {
    $.ajax({
        method: "POST",
        url: "http://localhost:5002/api/print",
        dataType: "json",
        data: { printerName: "Zebra", labelContent: label }
    })
}