//Blocks page from refreshing so it doesn't lose Secure State
window.preventF5 = function () {
    document.addEventListener("keydown", function (event) {
        if (event.key === "F5" || (event.ctrlKey && event.key === "r")) {
            event.preventDefault();
        }
    });
};
