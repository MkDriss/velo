function addStep() {
    const stepsContainer = document.getElementById("steps-container");
    const stepCount = stepsContainer.children.length + 1;

    const step = document.createElement("input-custom");
    step.setAttribute("id", "step-" + stepCount);

    const label = document.createElement("span");
    label.setAttribute("slot", "name");
    label.textContent = "Étape " + stepCount;
    step.appendChild(label);

    stepsContainer.appendChild(step);
}

function toggle_menu() {
    let menu = document.getElementById("menu");
    let mobileNav = document.getElementById("mobileNav");
    if (menu.classList.contains("inactive")) {
        menu.classList.replace("inactive", "active");
        mobileNav.classList.replace("mobileNavInactive", "mobileNavActive");
    }
    else {
        menu.classList.replace("active", "inactive");
        mobileNav.classList.replace("mobileNavActive", "mobileNavInactive");
    }
}

function fillExemple1(){
    departureInput = getInputComponent("departure");
    arrivalInput = getInputComponent("arrival");

    if (departureInput) {
        departureInput.value = "2 Rue Job 31000 Toulouse";
    }

    if (arrivalInput) {
        arrivalInput.value = "Cours Emile Zola 69100 Villeurbanne";
    }
}

function getInputComponent(id){
    return document.getElementById(id).shadowRoot.querySelector("input");
}