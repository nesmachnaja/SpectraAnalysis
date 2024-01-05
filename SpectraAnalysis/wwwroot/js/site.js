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
        var responceMsg = JSON.parse(xhr.responseText);
        if (xhr.status === 200) {
            responce.innerHTML = responceMsg.message;
            responce.dispatchEvent(new Event("change"));
            //analysisVisibilityControl();
        }
        else {
            responce.innerHTML = "Uploading failed. Status: ", responceMsg.message;
            responce.dispatchEvent(new Event("change"));
        }
    };

    xhr.send(formDataFile);
}

function uploadAbilityControl(event) {
    var spectraName = event.target;
    var okBtn = document.getElementById("uploadFile");
    if (spectraName.value == "")
        okBtn.disabled = true;
    else okBtn.disabled = false;
}

function stepAbilityControl(event) {
    var textfield = event.target;
    var accordingBtn = textfield.parentElement.getElementsByClassName("startStep")[0];
    if (textfield.value == "")
        accordingBtn.disabled = true;
    else accordingBtn.disabled = false;
}

function analysisVisibilityControl(event) {
    var message = event.target;
    var analysisWindow = document.getElementById("analysisWindow");
    if (message.innerHTML === "success") analysisWindow.style.display = "block";
    else analysisWindow.style.display = "none";
}

function startWaveletSmoothing() {
    var responceWaveletSmoothing = document.getElementById("responceWaveletSmoothing");
    var numOfIterations = document.getElementById("numOfIterations");
    var formData = new FormData();
    formData.append("numOfIterations", numOfIterations.value);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Home/SmoothingByWaveletHaar");

    xhr.onload = function () {
        var responceMsg = JSON.parse(xhr.responseText);
        if (xhr.status === 200) {
            responceWaveletSmoothing.innerHTML = responceMsg.message;
            responceWaveletSmoothing.dispatchEvent(new Event("change"));
        }
        else {
            responceWaveletSmoothing.innerHTML = "Status: ", responceMsg.message;
            responceWaveletSmoothing.dispatchEvent(new Event("change"));
        }
    };

    xhr.send(formData);
}

function startBaselineCorrection() {
    var responceIABaseline = document.getElementById("responceIABaseline");
    var threshold = document.getElementById("threshold");
    var formData = new FormData();
    formData.append("threshold", threshold.value);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Home/BaselineCorrection");

    xhr.onload = function () {
        var responceMsg = JSON.parse(xhr.responseText);
        if (xhr.status === 200) {
            responceIABaseline.innerHTML = responceMsg.message;
            responceIABaseline.dispatchEvent(new Event("change"));
        }
        else {
            responceIABaseline.innerHTML = "Status: ", responceMsg.message;
            responceIABaseline.dispatchEvent(new Event("change"));
        }
    };

    xhr.send(formData);
}