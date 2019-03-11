document.getElementById("assyProgress").value = 0;
let selectedProduct;

async function readBOMTap() {
    let selectedSerialNumber = document.getElementById("assSerialNumber").value
    if (selectedSerialNumber != null & selectedSerialNumber.length === 14) {
        console.log(selectedSerialNumber)
        $.ajax({
            method: "POST",
            url: "/Product/getProductBySN",
            dataType: "json",
            data: { serialnum: selectedSerialNumber }
        }).done(function (response) {
            selectedProduct = response;
            if (response.stationID === 1) {
                alert("Go to the Kitting station!")
            }
            if (response.stationID === 2) {
                console.log(response);
                getImagebySN(response);
                readNextPart(response);
            } else if (response.stationID === 3) {
                alert("Go to the Pack station!")
            }
            else if (response.stationID === 4) {
                alert("You are done!")
            }
        })
    } else {
        alert("Invalid Serial Number!");
        document.getElementById("partNumber").innerText = "";       
    }
}

function getImagebySN(p) {
    $.ajax({
        method: "POST",
        url: "/Product/getImageUrlBySN",
        dataType: "json",
        data: { p: p }
    }).done(function (response) {
        if (response != null) {
            document.getElementById("productimage").src = "/images/" + response.partName + ".png";
        }
    })
}

  function readNextPart(product) {
    let p = product;
       $.ajax({
         method: "POST",
         url: "/Assembly/getProgress",
         dataType: "json",
         data: { p: p }
       }).done(function (response) {
           document.getElementById("assyProgress").style.width = response + "%";
           if (response != 100) {
               getNextPart(p);
           }
           else {
               alert("Go to the Pack station!")
           }
        }
      )
}


 function getNextPart(p) {
    $.ajax({
        method: "POST",
        url: "/Assembly/getNextBom",
        dataType: "json",
        data: { p: p }
    }).done(function (response) {
        console.log(response.code)
        document.getElementById("nextpartlabel").innerText = "Read the code of  " + response.partName + " part!";
        document.getElementByID("partNumber").value = "";
        document.getElementByID("partNumber").innerText = " ";
        return response.code;
    })
}


function readPart() {
        $.ajax({
            method: "POST",
            url: "/Assembly/assemblyPart",
            dataType: "json",
            data: { sn: document.getElementById("assSerialNumber").value }
        }).done(function (response) {
            console.log(response);
            readNextPart(selectedProduct);
            document.getElementById("partNumber").value="";
            return response;
            })
}

function readPartTap() {
    let pn = document.getElementById("partNumber").value;
    $.ajax({
        method: "POST",
        url: "/Assembly/checkPart",
        dataType: "json",
        data: { sn: document.getElementById("assSerialNumber").value }
    }).done(function (response) {
        if (response != null) {
            console.log(response.code);
            if (response.code != pn) {
                alert("Wrong part number!")
                document.getElementById("partNumber").value = "";
            } else {
                readPart();
            }
        } else {
            console.log("error")
        }
    })
}