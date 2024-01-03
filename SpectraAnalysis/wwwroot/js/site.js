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
    var formDataFile = new FormData();
    formDataFile.append("file", fileInput.files[0]);

    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Home/UploadFile");
    xhr.send(formDataFile);
}