// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $("#uploadFile").on("click", function () {

        var fileInput = document.getElementById("spectraFile");
        sendFile(fileInput);

    });
});

function sendFile(fileInput) {
    var responce = document.getElementById("responce");
    var formDataFile = new FormData();
    formDataFile.append("file", fileInput.files[0]);
    formDataFile.append("spectraName", document.getElementById("spectraName").value);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Home/UploadFile");

    xhr.onload = function () {
        responceMsg = JSON.parse(xhr.responseText);
        if (xhr.status === 200) {
            responce.innerHTML = responceMsg.message;
        }
        else {
            responce.innerHTML = "Uploading failed. Status: ", responceMsg.message;
        }
    };

    xhr.send(formDataFile);
}

function uploadAbilityControl(event) {
    spectraName = event.target;
    okBtn = document.getElementById("uploadFile");
    if (spectraName.value == "")
        okBtn.disabled = true;
    else okBtn.disabled = false;
}