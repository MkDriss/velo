class InputCustom extends HTMLElement {

    constructor() {
        super();
        this.input = "";
        this.debounceTimer = null;
        this.attachShadow({ mode: "open" });
    }

    async connectedCallback() {
        const response = await fetch("./components/input-custom/input-template.html");
        const content = await response.text();
        const templateContent = new DOMParser()
            .parseFromString(content, "text/html")
            .querySelector("template").content;
        this.shadowRoot.appendChild(templateContent.cloneNode(true));

        this.inputElement = this.shadowRoot.querySelector("input");
        this.listElement = this.shadowRoot.querySelector("ul");

        this.inputElement.addEventListener("input", (e) => {
            this.input = e.target.value.trim();
            if (this.input === "") {
                this.clearList();
                return;
            }
            this.debounce();
        });

        document.addEventListener("click", (e) => {
            if (!this.contains(e.target)) this.clearList();
        });
    }

    debounce() {
        clearTimeout(this.debounceTimer);
        this.debounceTimer = setTimeout(() => {
            this.fetchSuggestions();
        }, 500);
    }

    async fetchSuggestions() {
        try {
            const res = await fetch(`https://api-adresse.data.gouv.fr/search/?q=${encodeURIComponent(this.input)}&limit=5`);
            const data = await res.json();
            const values = data.features;
            this.renderList(values);
        } catch (err) {
            console.error(err);
            this.clearList();
        }
    }

    renderList(values) {
        this.listElement.innerHTML = "";

        values.forEach((element) => {
            const li = document.createElement("li");
            li.textContent = element.properties.label;

            li.addEventListener("click", () => {
                this.inputElement.value = element.properties.label;
                this.input = element.properties.label;
                this.clearList();

                this.dispatchEvent(new CustomEvent("selection", { detail: element, bubbles: true }));
            });

            this.listElement.appendChild(li);
        });
    }

    clearList() {
        this.listElement.innerHTML = "";
    }

    static get observedAttributes() { return ['input']; }
}

customElements.define("input-custom", InputCustom);
