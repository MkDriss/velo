// Sidebar Component JavaScript
class SideBar extends HTMLElement {
    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
    }
    
    async connectedCallback() {
        const response = await fetch("./components/SideBar/SideBar.html");
        const content = await response.text();
        const templateContent = new DOMParser()
            .parseFromString(content, "text/html")
            .querySelector("template").content;
        this.shadowRoot.appendChild(templateContent.cloneNode(true));
        
        this.setupButtons();
    }
    
    setupButtons() {
        const buttons = this.shadowRoot.querySelectorAll('.sidebar-button');
        buttons.forEach(button => {
            button.addEventListener('click', (e) => {
                buttons.forEach(btn => btn.classList.remove('running'));
                button.classList.add('running');
                
                // Dispatch custom event with button id
                this.dispatchEvent(new CustomEvent('test-run', {
                    detail: { testId: button.id },
                    bubbles: true,
                    composed: true
                }));
            });
        });
    }
}

customElements.define('side-bar', SideBar);