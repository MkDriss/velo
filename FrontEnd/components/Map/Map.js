class __MapComponent__ extends HTMLElement {
    constructor() {
        super();
        this.mapContainer = document.createElement('div');
        this.mapContainer.style.width = '100%';
        this.mapContainer.style.height = '100%';
        this.appendChild(this.mapContainer);
        this.map = null;
        this.routesLayer = null;
    }
    
    async connectedCallback() {
        await this.loadLegend();
        await this.loadLeaflet();
        
        const width = this.offsetWidth;
        const height = this.offsetHeight;
        if (width > 0 && height > 0) {
            this.map = L.map(this.mapContainer, { zoomControl: false })
                .setView([48.8566, 2.3522], 13);
            
            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '© OpenStreetMap Contributors'
            }).addTo(this.map);
            
            this.routesLayer = L.featureGroup().addTo(this.map);
            
            L.control.zoom({ position: 'bottomright' }).addTo(this.map);
        }
    }
    
    async loadLegend() {
        const response = await fetch("./components/Map/legend.html");
        const content = await response.text();
        const templateContent = new DOMParser()
            .parseFromString(content, "text/html")
            .querySelector("template").content;
        this.appendChild(templateContent.cloneNode(true));
    }
    
    loadPath(data, color) {
        if (!this.map) return;
        const itineraire = L.geoJSON(data, {
            style: { color: color || 'blue', weight: 4 }
        }).addTo(this.routesLayer);
        setTimeout(() => {
            const bounds = this.routesLayer.getBounds();
            if (bounds.isValid()) {
                this.map.fitBounds(bounds);
            }
        }, 50);
    }
    
    clearPath(){
        this.routesLayer.clearLayers();
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
customElements.define('map-custom', __MapComponent__);