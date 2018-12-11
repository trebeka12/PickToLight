document.getElementById("assyProgress").value = 0;
let selectedProduct;

async function readBOMTap() {
    let selectedSerialNumber = document.getElementById("assSerialNumber").value
    if (selectedSerialNumber != null & selectedSerialNumber.length > 0) {
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
                console.log(response)
                readNextPart(response);
            } else if (response.stationID === 3) {
                alert("Go to the Pack station!")
            }
            else if (response.stationID === 4) {
                alert("You are done!")
            }
        })
    } else {
            document.getElementById("nextpartlabel").innerText = "Invalid SerialNumber! - " + document.getElementById("partNumber").value;
            document.getElementById("partNumber").innerText = "";       
    }
}

  function readNextPart(product) {
    let p = product;
       $.ajax({
         method: "POST",
         url: "/Assembly/getProgress",
         dataType: "json",
         data: { p: p }
      }).done( function (response) {
          document.getElementById("assyProgress").style.width="response%"
          getNextPart(p);
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
        return response.code;
    })
}


function readPartTap() {

    var d = $.Deferred();
    $.ajax({
        method: "POST",
        url: "/Assembly/assemblyPart",
        dataType: "json",
        data: { sn: document.getElementById("assSerialNumber").value }
    }).done(function (response) {
        console.log(response);
        getNextPart(selectedProduct);
        d.resolve(response);
        return response
        })


      //let selectedPart= getPart(document.getElementById("partNumber").value);
     // console.log(selectedPart)
}



function ajaxStart() {
    $("#imgProg").show();
    $("#imgProg").css("display", "block");
};
function ajaxStop() {
    $("#imgProg").hide();
    $("#imgProg").css("display", "none");
};