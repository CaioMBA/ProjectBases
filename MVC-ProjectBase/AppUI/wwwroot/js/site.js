function showModal(id) {
    var modal = document.getElementById(id);
    modal.style.display = "flex";
    modal.hidden = false;
}

function closeModal(id) {
    var modal = document.getElementById(id);
    modal.style.display = "none";
    modal.hidden = true;
}

function showOrCloseModal(id) {
    let modal = document.getElementById(id);
    let displayed = modal.style.display;
    let hidden = modal.hidden;
    if (displayed == "none" || hidden == true) {
        showModal(modal.id);
    }
    else {
        closeModal(modal.id);
    }
}

function showLoading() {
    showModal('loading-square');
}

function hideLoading() {
    closeModal('loading-square');
}

function setItemInDropdown(dropdownId, value) {
    var dropdown = document.getElementById(dropdownId);
    for (var i = 0; i < dropdown.options.length; i++) {
        if (dropdown.options[i].value == value) {
            dropdown.selectedIndex = i;
            break;
        }
    }
}

function showToast(type, message) {
    const toastContainer = document.getElementById("toast-container");

    const toast = document.createElement("div");
    toast.className = `toast-notification ${type}`;

    const icon = document.createElement("span");
    icon.className = "icon";
    icon.innerHTML = getIcon(type);

    const text = document.createElement("span");
    text.textContent = message;

    const closeBtn = document.createElement("button");
    closeBtn.className = "close-btn";
    closeBtn.innerHTML = "&times;";
    closeBtn.onclick = () => removeToast(toast);

    const progress = document.createElement("div");
    progress.className = "progress";

    toast.append(icon, text, closeBtn, progress);
    toastContainer.appendChild(toast);

    let progressWidth = 100;
    const interval = setInterval(() => {
        progressWidth -= 1;
        progress.style.width = `${progressWidth}%`;
        if (progressWidth <= 0) {
            clearInterval(interval);
            removeToast(toast);
        }
    }, 20);
}

function removeToast(toast) {
    toast.style.animation = "slideOut 0.5s forwards"; // Animação de saída
    setTimeout(() => toast.remove(), 500); // Remove após a animação
}

function getIcon(type) {
    switch (type) {
        case "success":
            return "✅";
        case "error":
            return "❌";
        case "warning":
            return "⚠️";
        case "info":
            return "ℹ️";
        default:
            return "";
    }
}