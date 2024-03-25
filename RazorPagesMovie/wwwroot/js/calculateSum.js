
document.addEventListener("DOMContentLoaded", function() {
    const sumButton = document.getElementById("sumButton");
    const sumForm = document.getElementById("sumForm");
    const calculateButton = document.getElementById("calculate");
    const sumResult = document.getElementById("sumResult");

    sumButton.addEventListener("click", function() {
        sumForm.classList.toggle("hidden");
    });

    calculateButton.addEventListener("click", function() {
        const num1 = parseFloat(document.getElementById("num1").value);
        const num2 = parseFloat(document.getElementById("num2").value);

        const sum = num1 + num2;
        sumResult.textContent = "The Sum of " + num1 + " and " + num2 + " is : " + sum;
    });
});
