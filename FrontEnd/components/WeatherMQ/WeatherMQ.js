import { Client } from 'https://cdn.jsdelivr.net/npm/@stomp/stompjs@7.0.0/+esm';

class __WeatherMQ__ extends HTMLElement {
    constructor() {
        super();
        this.queue = "weatherQueueVelo";
        this.client = null;
        this.attachShadow({ mode: "open" });
        this.message = "";
    }
    
    async connectedCallback() {
        
    // fetch le fichier HTML
    const response = await fetch("./components/WeatherMQ/WeatherMQ.html");
    const content = await response.text();

    // parser et récupérer le template
    const templateContent = new DOMParser()
        .parseFromString(content, "text/html")
        .querySelector("template").content;

    // injecter dans le shadow DOM
    this.shadowRoot.appendChild(templateContent.cloneNode(true));

    // Références DOM
    this.icon = this.shadowRoot.getElementById("icon");
    this.label = this.shadowRoot.getElementById("label");

        // Références DOM
        this.icon = this.shadowRoot.getElementById("icon");
        this.label = this.shadowRoot.getElementById("label");
        
        this.startConsumer();

    }
    
    startConsumer() {
        this.client = new Client({
            brokerURL: "ws://localhost:61614/stomp",
            reconnectDelay: 3000,
            onConnect: () => {
                console.log(`[WeatherMQ] Connecté à ActiveMQ — Queue: ${this.queue}`);
                this.client.subscribe(`/queue/${this.queue}`, (msg) => {
                    console.log("[WeatherMQ] Message reçu :", msg.body);
                    this.updateEvent(msg.body);
                    this.message = msg.body;                    
                    this.dispatchEvent(new CustomEvent('eventMQ', {
                        detail: { body: msg.body },
                        bubbles: true,
                        composed: true
                    }));
                });
            },
            onStompError: (frame) => {
                console.error("[WeatherMQ] Erreur STOMP :", frame.headers['message']);
            }
        });
        this.client.activate();
    }
    
    updateEvent(message) {
        if(message === "sun"){
            this.label.textContent = "Soleil";
            this.icon.innerHTML = '<img src="./assets/icons/sun.png" alt="Soleil" style="width: 48px; height: 48px;">';
        }
        else if(message === "cloud"){
            this.label.textContent = "Pluie";
            this.icon.innerHTML = '<img src="./assets/icons/rain.png" alt="Pluie" style="width: 48px; height: 48px;">';
        }
        else if(message === "god"){
            this.label.textContent = "Appel de Dieu"
            this.icon.innerHTML = '<img src="./assets/icons/god.png" alt="Appel de Dieu" style="width: 48px; height: 48px;">';
        }
        else{
            this.label.textContent = "...";
            this.icon.innerHTML = '';
        }
    }
    
    disconnectedCallback() {
        if (this.client) {
            this.client.deactivate();
            console.log("[WeatherMQ] Déconnecté.");
        }
    }

}

customElements.define('weather-mq', __WeatherMQ__);