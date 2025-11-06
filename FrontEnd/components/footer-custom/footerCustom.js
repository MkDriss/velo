

class FooterCustom extends HTMLElement {

    constructor() {
        super();
        this.attachShadow({ mode: "open" });
    }

    async connectedCallback() {
        let response = (await fetch("components/footer-custom/footer-template.html"));
        let content = await response.text();

        let templateContent = new DOMParser().parseFromString(content, "text/html").querySelector("template").content;

        this.shadowRoot.appendChild(templateContent.cloneNode(true));
    }
}

customElements.define("footer-custom", FooterCustom);

