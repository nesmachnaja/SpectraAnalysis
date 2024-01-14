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
    var spectraId = document.getElementById("spectraId");
    var formDataFile = new FormData();
    formDataFile.append("file", fileInput.files[0]);
    formDataFile.append("spectraName", document.getElementById("spectraName").value);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Home/UploadFile");

    xhr.onload = function () {
        var responceMsg = JSON.parse(xhr.responseText);
        if (xhr.status === 200) {
            responce.innerHTML = responceMsg.message;
            spectraId.innerHTML = responceMsg.spectra_id;
            responce.dispatchEvent(new Event("change"));
            analysisVisibilityControl();
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
    formData.append("spectraId", document.getElementById("spectraId").innerHTML);

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
    formData.append("numOfIterations", document.getElementById("numOfIterations").value);
    formData.append("spectraId", document.getElementById("spectraId").innerHTML);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Home/BaselineCorrection");

    xhr.onload = function () {
        var responceMsg = JSON.parse(xhr.responseText);
        if (xhr.status === 200) {
            document.getElementById("baselineId").innerHTML = responceMsg.baseline_id;
            responceIABaseline.innerHTML = responceMsg.message;
            responceIABaseline.dispatchEvent(new Event("change"));
        }
        else {
            document.getElementById("baselineId").innerHTML = responceMsg.baseline_id;
            responceIABaseline.innerHTML = "Status: ", responceMsg.message;
            responceIABaseline.dispatchEvent(new Event("change"));
        }
    };

    xhr.send(formData);
}

function simulationStart(event) {
    //var analysisWindow = document.getElementById("analysisWindow");
    //var currentAnalysisStep = event.target.parentElement.parentElement;
    //var analysisSteps = Array.from(analysisWindow.children);

    //var currentStepNum = analysisSteps.indexOf(currentAnalysisStep);
    //var currentStepNum = analysisWindow.children.indexOf(currentAnalysisStep);

    var formData = new FormData();
    formData.append("baselineId", document.getElementById("baselineId").innerHTML);
    formData.append("threshold", document.getElementById("threshold").value);
    formData.append("numOfIterations", document.getElementById("numOfIterations").value);
    formData.append("spectraId", document.getElementById("spectraId").innerHTML);

    var responceSimulation = document.getElementById("responceSimulation");
    var responceDenoising = document.getElementById("responceDenoising");

    if (event.target.innerHTML === "success") {
        responceSimulation.innerHTML = "started";
        responceDenoising.innerHTML = "started";

        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/Home/SimulateAndDenoise");
        xhr.onload = function () {
            var responceMsg = JSON.parse(xhr.responseText);
            if (xhr.status === 200) {
                responceSimulation.innerHTML = responceMsg.message;
                responceSimulation.dispatchEvent(new Event("change"));
                responceDenoising.innerHTML = responceMsg.message;
                responceDenoising.dispatchEvent(new Event("change"));

                if (responceMsg.message === "success")
                    responcePeakDetection.innerHTML = "started";
            }
            else {
                responceSimulation.innerHTML = "Status: ", responceMsg.message;
                responceSimulation.dispatchEvent(new Event("change"));
                responceDenoising.innerHTML = "Status: ", responceMsg.message;
                responceDenoising.dispatchEvent(new Event("change"));
            }
        };

        xhr.send(formData);
    }
}

function peakDetectionStart(event) {
    var formData = new FormData();
    formData.append("baselineId", document.getElementById("baselineId").innerHTML);
    formData.append("threshold", document.getElementById("threshold").value);
    formData.append("numOfIterations", document.getElementById("numOfIterations").value);
    formData.append("spectraId", document.getElementById("spectraId").innerHTML);

    var responcePeakDetection = document.getElementById("responcePeakDetection");

    if (event.target.innerHTML === "success") {
        responcePeakDetection.innerHTML = "started";

        var xhr = new XMLHttpRequest();
        xhr.open("POST", "/Home/DetectPeaks");
        xhr.onload = function () {
            var responceMsg = JSON.parse(xhr.responseText);
            if (xhr.status === 200) {
                responcePeakDetection.innerHTML = responceMsg.message;
                responcePeakDetection.dispatchEvent(new Event("change"));
            }
            else {
                responcePeakDetection.innerHTML = "Status: ", responceMsg.message;
                responcePeakDetection.dispatchEvent(new Event("change"));
            }
        };

        xhr.send(formData);
    }
}