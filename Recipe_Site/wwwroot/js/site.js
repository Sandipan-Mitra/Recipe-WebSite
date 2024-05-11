// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});
document.addEventListener("DOMContentLoaded", function () {
    var element = document.getElementById("preimage");
    if (element) {
        element.style.display = "none";
    }
});
function loadfile(event) {
    var output = document.getElementById('preimage');
    output.src = URL.createObjectURL(event.target.files[0]);
    output.style.display = "block";
    output.style.width = "130px";
};
function loadfilerecipepreview(event) {
    var output = document.getElementById('preimage');
    output.src = URL.createObjectURL(event.target.files[0]);
    output.style.display = "block";
    output.style.width = "250px";
    output.style.height = "180px";
};