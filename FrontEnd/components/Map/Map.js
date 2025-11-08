class MapComponent extends HTMLElement {
    constructor() {
        super();
        this.mapContainer = document.createElement('div');
        this.mapContainer.style.width = '100%';
        this.mapContainer.style.height = '100%';
        this.appendChild(this.mapContainer);
        this.map = null;
        this.routesLayer = null;
    }

    connectedCallback() {
        this.loadLeaflet().then(() => {
            const width = this.offsetWidth;
            const height = this.offsetHeight;
            if (width > 0 && height > 0) {
                // Initialize map WITHOUT default zoom control
                this.map = L.map(this.mapContainer, {
                    zoomControl: false
                }).setView([48.8566, 2.3522], 13);
                
                // Add tile layer
                L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                    attribution: '© OpenStreetMap Contributors'
                }).addTo(this.map);
                
                // Add zoom control in bottom right
                L.control.zoom({
                    position: 'bottomright'
                }).addTo(this.map);
            }
        });
    }

    loadPath(data, color) {
    const itineraire = L.geoJSON(data, {
      style: {
        color: color,
        weight: 4
      }
    }).addTo(routesLayer);

    map.fitBounds(itineraire.getBounds());
}

    async loadLeaflet() {
        if (!window.L) {
            if (!document.getElementById('leaflet-css')) {
                const link = document.createElement('link');
                link.id = 'leaflet-css';
                link.rel = 'stylesheet';
                link.href = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.css';
                document.head.appendChild(link);
            }
            await new Promise(resolve => {
                const script = document.createElement('script');
                script.src = 'https://unpkg.com/leaflet@1.9.4/dist/leaflet.js';
                script.onload = resolve;
                document.head.appendChild(script);
            });
        }
    }
}

customElements.define('map-custom', MapComponent);
