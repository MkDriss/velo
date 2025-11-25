import { AutocompleteManager } from '../RouteInput/autoCompleteManager.js';
import { RouteService } from '../RouteInput/routeService.js';
import { EventDispatcher } from '../RouteInput/eventDispatcher.js';

class RouteInput extends HTMLElement {
    constructor() {
        super();
        this.attachShadow({ mode: 'open' });
        this.event = "";
        
        // Initialize managers
        this.autocompleteManager = new AutocompleteManager(this.shadowRoot);
        this.routeService = new RouteService();
        this.eventDispatcher = new EventDispatcher(this);
    }

    setEvent(message) {
        this.event = message;
    }
    
    async connectedCallback() {
        await this.loadTemplate();
        this.initializeElements();
        this.setupEventListeners();
    }

    async loadTemplate() {
        const response = await fetch("./components/RouteInput/RouteInput.html");
        const content = await response.text();
        const templateContent = new DOMParser()
            .parseFromString(content, "text/html")
            .querySelector("template").content;
        this.shadowRoot.appendChild(templateContent.cloneNode(true));
    }

    initializeElements() {
        this.startInput = this.shadowRoot.getElementById('start');
        this.endInput = this.shadowRoot.getElementById('end');
        this.waypointInput = this.shadowRoot.getElementById('waypoint');
        this.addWaypointButton = this.shadowRoot.getElementById('add-waypoint');
        this.waypointGroup = this.shadowRoot.getElementById('waypoint-group');
        this.searchButton = this.shadowRoot.getElementById('search-button');
        this.parisianModeCheckbox = this.shadowRoot.getElementById('parisian-mode');
        this.pilgrimModeCheckbox = this.shadowRoot.getElementById('pilgrim-mode');
    }

    setupEventListeners() {
        this.autocompleteManager.setup(this.startInput, this.endInput, this.waypointInput);
        this.setupWaypointToggle();
        this.setupSearchButton();
        this.setupModeToggles();
    }

    setupModeToggles() {
        this.parisianModeCheckbox.addEventListener('change', () => {
            if (this.parisianModeCheckbox.checked) {
                this.pilgrimModeCheckbox.checked = false;
            }
        });
        
        this.pilgrimModeCheckbox.addEventListener('change', () => {
            if (this.pilgrimModeCheckbox.checked) {
                this.parisianModeCheckbox.checked = false;
            }
        });
    }

    setupWaypointToggle() {
        if (this.addWaypointButton) {
            this.addWaypointButton.addEventListener("click", () => {
                if (this.waypointGroup.style.display === 'none') {
                    this.waypointGroup.style.display = 'flex';
                    this.addWaypointButton.textContent = '- Retirer l\'étape';
                    this.addWaypointButton.style.background = '#e2bcbcff';
                } else {
                    this.waypointGroup.style.display = 'none';
                    this.addWaypointButton.textContent = '+ Ajouter une étape';
                    this.addWaypointButton.style.background = '#f5f5f5';
                    this.waypointInput.value = '';
                    this.autocompleteManager.clearList(this.autocompleteManager.waypointList);
                }
            });
        }
    }

    setupSearchButton() {
        if (this.searchButton) {
            this.searchButton.addEventListener("click", () => {
                this.handleSearch();
            });
        }
    }

    async handleSearch() {
        this.setLoadingState(true);
        this.eventDispatcher.dispatchRouteEvent('search-btn-pressed', "");
        
        const addresses = this.getAllAddresses();
        const parisianMode = this.parisianModeCheckbox.checked;
        const pilgrimMode = this.pilgrimModeCheckbox.checked;
        
        try {
            await this.fetchRoute(addresses, parisianMode, pilgrimMode);
        } catch (error) {
            console.error('Search error:', error);
        } finally {
            setTimeout(() => this.setLoadingState(false), 500);
        }
    }

    setLoadingState(isLoading) {
        const buttonText = this.searchButton.querySelector('.button-text');
        const loader = this.searchButton.querySelector('.loader');
        
        buttonText.style.display = isLoading ? 'none' : 'block';
        loader.style.display = isLoading ? 'block' : 'none';
        this.searchButton.disabled = isLoading;
    }

    async fetchRoute(addresses, parisianMode, pilgrimMode) {
        const { start, end, waypoint } = addresses;
        
        if (this.event === "cloud") {
            await this.handleCloudMode(start, end, waypoint);
        } else if (pilgrimMode || this.event === "god") {
            await this.handlePilgrimMode(start, end, waypoint);
        } else if (parisianMode) {
            await this.handleParisianMode(start, end);
        } else {
            await this.handleNormalMode(start, end, waypoint);
        }
    }

    async handleCloudMode(start, end, waypoint) {
        alert("Attention : conditions météorologiques difficiles (pluie). Soyez prudent lors de votre trajet !");
        
        if (!waypoint) {
            const walk = await this.routeService.fetchWalk(start, end);
            this.eventDispatcher.dispatchRouteEvent('route-display', walk);
            this.eventDispatcher.dispatchMarkerEvent(walk, "departure");
            this.eventDispatcher.dispatchMarkerEvent(walk, "arrival");
        } else {
            const firstLeg = await this.routeService.fetchWalk(start, waypoint);
            this.eventDispatcher.dispatchRouteEvent('route-display', firstLeg);
            this.eventDispatcher.dispatchMarkerEvent(firstLeg, "departure");
            
            const secondLeg = await this.routeService.fetchWalk(waypoint, end);
            this.eventDispatcher.dispatchRouteEvent('route-display', secondLeg);
            this.eventDispatcher.dispatchMarkerEvent(secondLeg, "arrival");
        }
    }

    async handlePilgrimMode(start, end, waypoint) {
        if (waypoint) {
            alert("Le mode pèlerin ne supporte pas les étapes intermédiaires.");
            return;
        }

        const walk1 = await this.routeService.fetchWalk(start, "Saint Jacques de Compostelle");
        this.eventDispatcher.dispatchRouteEvent('route-display', walk1);
        this.eventDispatcher.dispatchMarkerEvent(walk1, "departure");
        
        const pel = await this.routeService.fetchWalk("Saint Jacques de Compostelle", "Boulevard de la Grotte 65100 Lourdes");
        this.eventDispatcher.dispatchRouteEvent('route-display', pel);
        
        const walk2 = await this.routeService.fetchBike("Boulevard de la Grotte 65100 Lourdes", end);
        this.eventDispatcher.dispatchRouteEvent('route-display', walk2);
        this.eventDispatcher.dispatchMarkerEvent(walk2, "arrival");
    }

    async handleParisianMode(start, end) {
        const toSeine = await this.routeService.fetchBikeSeine(start);
        this.eventDispatcher.dispatchRouteEvent('route-display', toSeine);
        this.eventDispatcher.dispatchMarkerEvent(toSeine, "departure");
        
        const toEnd = await this.routeService.fetchItinerary("Quai de la Seine 75019 Paris", end);
        this.eventDispatcher.dispatchRouteEvent('route-display', toEnd);
        this.eventDispatcher.dispatchMarkerEvent(toEnd, "arrival");
    }

    async handleNormalMode(start, end, waypoint) {
        if (!waypoint) {
            const itin = await this.routeService.fetchItinerary(start, end);
            this.eventDispatcher.dispatchRouteEvent('route-display', itin);
            this.eventDispatcher.dispatchMarkerEvent(itin, "departure");
            this.eventDispatcher.dispatchMarkerEvent(itin, "arrival");
        } else {
            const firstLeg = await this.routeService.fetchItinerary(start, waypoint);
            this.eventDispatcher.dispatchRouteEvent('route-display', firstLeg);
            this.eventDispatcher.dispatchMarkerEvent(firstLeg, "departure");
            
            const secondLeg = await this.routeService.fetchItinerary(waypoint, end);
            this.eventDispatcher.dispatchRouteEvent('route-display', secondLeg);
            this.eventDispatcher.dispatchMarkerEvent(secondLeg, "arrival");
        }
    }

    getAllAddresses() {
        return {
            start: this.startInput?.value || '',
            waypoint: this.waypointInput?.value || null,
            end: this.endInput?.value || ''
        };
    }

    setAddresses(start, end, waypoint = null) {
        this.startInput.value = start;
        this.endInput.value = end;
        
        if (waypoint) {
            if (this.waypointGroup.style.display === 'none') {
                this.addWaypointButton.click();
            }
            this.waypointInput.value = waypoint;
        } else {
            if (this.waypointGroup.style.display !== 'none') {
                this.addWaypointButton.click();
            }
        }
    }

    setParisianMode(val) {
        this.parisianModeCheckbox.checked = val;
    }

    setPilgrimMode(val) {
        this.pilgrimModeCheckbox.checked = val;
    }

    istPilgrimModeActived() {
        return this.pilgrimModeCheckbox.checked;
    }
}

customElements.define('route-input', RouteInput);