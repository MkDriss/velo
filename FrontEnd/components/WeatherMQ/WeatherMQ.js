import { Client } from 'https://cdn.jsdelivr.net/npm/@stomp/stompjs@7.0.0/+esm';

class __WeatherMQ__ extends HTMLElement {
    constructor() {
        super();
        this.queue = "weather";
        this.client = null;
    }
    
    connectedCallback() {
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
                    
                    // Dispatch a custom event with the message data
                    this.dispatchEvent(new CustomEvent('weather-message', {
                        detail: { body: msg.body },
                        bubbles: true,
                        composed: true
                    }));
                });
            },
            onStompError: (frame) => {
                console.error("[WeatherMQ] Erreur STOMP :", frame.headers['message']);
                console.error("[WeatherMQ] Détails :", frame.body);
            }
        });
        this.client.activate();
    }
    
    disconnectedCallback() {
        if (this.client) {
            this.client.deactivate();
            console.log("[WeatherMQ] Déconnecté.");
        }
    }
}

customElements.define('weather-mq', __WeatherMQ__);