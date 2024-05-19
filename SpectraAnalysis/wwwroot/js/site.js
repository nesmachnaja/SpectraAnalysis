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


$(document).ready(function () {

    $.ajax({
        url: "/Home/VisualizeSpectra",
        type: "GET",
        dataType: "json",
        success: function (response) {
            var baseline_correction_data = JSON.parse(response.baseline_correction_data.value);
            var max_peak_height = Number(response.max_peak_height.value);

            response = null;

            var rCoeff = 255 / (max_peak_height / 1000);
            var gbCoeff = 255 / (max_peak_height / 100);
            var maxRed = Math.floor(max_peak_height / 100);

            var canvas = document.getElementById("myCanvas");
            var ctx = canvas.getContext("2d");
            ctx.imageSmoothingEnabled = true;

            canvas.height = baseline_correction_data.length * 0.2;
            canvas.width = baseline_correction_data[0].length;

            for (var i = 0; i < baseline_correction_data.length; i++) {
                for (var j = 0; j < baseline_correction_data[i].length; j++) {
                    var x = j;
                    var y = i * 0.2; // Math.floor(i / width);

                    //var a = Math.floor(baseline_correction_data[i][j] * 100000 % 1000);

                    /*var color = `rgb(${Math.floor(1 / 3.91 /*for conversion to an eight-bit representation/ * (baseline_correction_data[i][j] * 10000 / 100000))}, 
                                     ${Math.floor(1 / 3.91 /*for conversion to an eight-bit representation/ * (baseline_correction_data[i][j] * 100 % 1000))}, 
                                     ${Math.floor(1 / 3.91 /*for conversion to an eight-bit representation/ * (baseline_correction_data[i][j] * 100000 % 1000))})`;*/


                    /*var color = `rgb(${Math.floor(baseline_correction_data[i][j] / 1000 * 28.33)}, 
                                     ${baseline_correction_data[i][j] > 999 ? Math.floor((baseline_correction_data[i][j] / 100) * 2.57) : Math.floor(Math.floor(baseline_correction_data[i][j] / 10) % 100 * 2.57)}, 
                                     ${baseline_correction_data[i][j] > 999 ? Math.floor((99 - baseline_correction_data[i][j] / 100) * 2.57) : Math.floor((baseline_correction_data[i][j] * 100 % 1000) / 3.91)})`;*/

                    var color = `rgb(${Math.floor(baseline_correction_data[i][j] / 1000 * rCoeff)}, 
                                     ${
                                     //baseline_correction_data[i][j] <= 999 ? 
                                     //Math.floor(Math.floor(baseline_correction_data[i][j] / 10) % 100 * 2.57) :
                                     Math.floor((baseline_correction_data[i][j] / 100) * gbCoeff) > Math.floor(Math.floor(baseline_correction_data[i][j] / 10) % 100 * 2.57) ?
                                     Math.floor((baseline_correction_data[i][j] / 100) * gbCoeff) :
                                     Math.floor(Math.floor(baseline_correction_data[i][j] / 10) % 100 * 2.57)}, 
                                     ${//baseline_correction_data[i][j] <= 999 ||
                                     Math.floor((maxRed - baseline_correction_data[i][j] / 100) * gbCoeff) > Math.floor((baseline_correction_data[i][j] * 100 % 1000) / 3.91) ?
                                     Math.floor((baseline_correction_data[i][j] * 100 % 1000) / 3.91) :
                                     Math.floor((maxRed - baseline_correction_data[i][j] / 100) * gbCoeff)})`;

                    ctx.fillStyle = color;
                    ctx.fillRect(x, y, 1, 1);
                }
            }

            //canvas.transform(1, 0, 0, 0.2, 0, 0);
            //ctx.scale(1, 0.2);
        },
        error: function (xhr, status, error) {
        }
    });

    //function visualizeSpectra() {
    /*var canvas = document.getElementById("myCanvas");
    var ctx = canvas.getContext("2d");

    var width = 12;
    var height = 12;

    var rgbData = [
        [255, 0, 0],
        [0, 255, 0],
        [0, 0, 255],
        [255, 0, 0],
        [0, 255, 0],
        [0, 0, 255],
        [255, 0, 0],
        [0, 255, 0],
        [0, 0, 255],
        [255, 0, 0],
        [0, 255, 0],
        [0, 0, 255],
        [255, 0, 0],
        [0, 255, 0],
        [0, 0, 255],
        [255, 0, 0],
        [0, 255, 0],
        [0, 0, 255],
        [255, 0, 0],
        [0, 255, 0],
        [0, 0, 255],
        [255, 0, 0],
        [0, 255, 0],
        [0, 0, 255],
        [255, 0, 0],
        [0, 255, 0],
        [0, 0, 255],
        [255, 0, 0],
        [0, 255, 0],
        [0, 0, 255],
        [255, 0, 0],
        [0, 255, 0],
        [0, 0, 255],
        [255, 0, 0],
        [0, 255, 0],
        [0, 0, 255],
        [255, 0, 0],
        [0, 255, 0],
        [0, 0, 255]
    ];

    for (var i = 0; i < rgbData.length; i++) {
        var x = i % width;
        var y = Math.floor(i / width);

        var color = `rgb(${rgbData[i][0]}, ${rgbData[i][1]}, ${rgbData[i][2]})`;

        ctx.fillStyle = color;
        ctx.fillRect(x, y, 1, 1);
    }*/
});
