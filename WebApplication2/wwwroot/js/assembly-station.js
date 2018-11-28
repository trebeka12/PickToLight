

async function readBOMTap() {
    let selectedSerialNumber = document.getElementById("assSerialNumber").value
    if (selectedSerialNumber != null & selectedSerialNumber.length > 0) {
        console.log(selectedSerialNumber)
        $.ajax({
            method: "POST",
            url: "/Product/getProductBySN",
            dataType: "json",
            data: { serialnum: selectedSerialNumber }
        }).done(async function (response) {
            let product = await readNextPart(response);
            return product;
        })
    } else {
            document.getElementById("nextpartlabel").innerText = "Invalid SerialNumber! - " + document.getElementById("partNumber").value;
            document.getElementById("partNumber").innerText = "";       
    }
}


async function readNextPart(product) {
    let p = product;
       $.ajax({
         method: "POST",
         url: "/Assembly/getNextBOM",
         dataType: "json",
         data: { p: p }
      }).done(function (response) {
          if (response != null) {
              document.getElementById("nextpartlabel").innerText = "Read the code of  " + response.partName + "  part!";
              document.getElementById("partNumber").innerText = "";
          } else {
              document.getElementById("nextpartlabel").innerText = "BOM is complete-";
              document.getElementById("partNumber").innerText = "";
          }
        }
      )
}


 function readPartTap() {
    let selectedPart =  getPart();
    if (selectedPart != null) {
        if (document.getElementById("assSerialNumber").value == null) {
            document.getElementById("partlabel").innerText = "Read the SerialNumber first!";
            document.getElementById("partNumber").value = "";
        }
        if (document.getElementById("partNumber").value != selectedPart) {
            document.getElementById("partlabel").innerText = "Partcode is wrong!";
            document.getElementById("partNumber").value = "";
        } else {
            if (document.getElementById("assSerialNumber").value != null && selectedPart != null) {
                console.log("ok")
            }
        }
    } else {
        console.log("error")
    }
}

async function getPart() {
    let selectedPN = document.getElementById("partNumber").value;
    $.ajax({
        method: "POST",
        url: "/Part/getPartByPN",
        dataType: "json",
        data: { partnum: selectedPN }
    }).done(function (response) {
        return response.code;
    })
}