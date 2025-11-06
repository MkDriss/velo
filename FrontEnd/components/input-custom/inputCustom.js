

class InputCustom extends HTMLElement {

    constructor() {
        super();
        this.input = "";
        this.debounceTimer = null;
        this.attachShadow({ mode: "open" });

    }

    async connectedCallback() {
        let response = (await fetch("components/input-custom/input-template.html"));
        let content = await response.text();

        let templateContent = new DOMParser().parseFromString(content, "text/html").querySelector("template").content;

        this.shadowRoot.appendChild(templateContent.cloneNode(true));

        this.inputElement = this.shadowRoot.querySelector("input");
        this.listElement = this.shadowRoot.querySelector("ul");

        this.inputElement.addEventListener("keyup", (e) => {
            this.input = e.target.value;
            if (this.input === "") return;
            this.debounce();
        });
    }

    debounce() {
        clearTimeout(this.debounceTimer);
        
        this.debounceTimer = setTimeout(() => {
            let data = fetch(`https://api-adresse.data.gouv.fr/search/?q=${this.input}&limit=5`);
            data
                .then((res) => res.json())
                .then((data) => {
                    const values = data.features;
                    console.log(values);

                    this.listElement.innerHTML = "";
                    values.forEach(element => {
                        let li = document.createElement("li");
                        li.textContent = element.properties.label;
                        this.listElement.appendChild(li);
                    });
                })
        }, 500);
    }

    static get observedAttributes() { return ['input']; }

    //    attributeChangedCallback(property, newValue) { this[property] = newValue; console.log(this.input) }
}

customElements.define("input-custom", InputCustom);

