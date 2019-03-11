async function kittingSNTap() {
    let selectedSerialNumber = document.getElementById("serialNumber").value
    if (selectedSerialNumber != null & selectedSerialNumber.length === 14) {            
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
                    alert("Pack is complete!")
                    document.getElementById("kittinglabel").innerText = ""
s                }
            })
    } else {
        alert("Invalid serial number!");
        document.getElementById("kittinglabel").innerText = "" 
    }
}

async function getWeight() {
    var sn = document.getElementById("serialNumber").value;
    if (sn.length === 0) {
        alert("Read the serial number of product!")
    } else {
        $.ajax({
            method: "POST",
            url: "/Kitting/getWeightBySN",
            dataType: "json",
            data: { sn: sn }
        }).done(function (response) {
            var tWeight = response.weight;
            readWeight(tWeight);
        })
    }
}

async function readWeight(tWeight){
    var sn = document.getElementById("serialNumber").value;
    $.ajax({
        method: "POST",
        url: "/Kitting/readWeight",
        dataType: "json",
        data: { sn: sn }
    }).done(function (response) {
        var weight = response;
        if (weight === -1 || tWeight === 0) {
            console.log("Error");
        }
        else if(weight >= tWeight + 1 || weight <= tWeight - 1) {
            alert("Your weight (" + weight + "g) is out of target weight (" + tWeight +  "g) !")
        }
        else if (weight === 0) {
            alert("Put the kitted parts on the scale!")
        }
        else {
            setWeight(weight);
        }
    })    
}

async function setWeight(w) {
    var sn = document.getElementById("serialNumber").value;
    $.ajax({
        method: "POST",
        url: "/Kitting/setWeight",
        dataType: "json",
        data: { sn: sn, w:w }
    }).done(function () {
        if (response = true) {
            alert("Go to the Assembly station!")
        } else {
            console.log("error")
        }
    })
}

async function kittingSN(product) {
    let p = product;
    console.log(p)
    $.ajax({
        method: "POST",
        url: "/Kitting/GetBoms",      
        dataType: "json",
        data: {p: p}
    }).done(function (response) {
        if (response != null) {
            console.log(response)
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
        } else {
            alert('Part shortage');
        }
   })
}