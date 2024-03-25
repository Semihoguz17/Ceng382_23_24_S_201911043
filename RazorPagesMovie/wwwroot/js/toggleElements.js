
document.addEventListener("DOMContentLoaded", function() {
    const toggleButton = document.getElementById("toggleButton");
    const elementsToToggle = document.querySelectorAll(".toggle-element");

    toggleButton.addEventListener("click", function() {
        elementsToToggle.forEach(element => {
            element.classList.toggle("hidden");
        });
    });
});
