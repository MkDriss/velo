class RouteInput extends HTMLElement {
    constructor() {
        super();
        this.input = "";
        this.debounceTimer = null;
        this.attachShadow({ mode: 'open' });
    }
    
    async connectedCallback() {
        const response = await fetch("./components/RouteInput/RouteInput.html");
        const content = await response.text();
        const templateContent = new DOMParser()
            .parseFromString(content, "text/html")
            .querySelector("template").content;
        this.shadowRoot.appendChild(templateContent.cloneNode(true));
        
        this.inputElement = this.shadowRoot.querySelector("input");
        this.listElement = this.shadowRoot.querySelector("ul");
        
        // Get elements
        this.addWaypointButton = this.shadowRoot.getElementById('add-waypoint');
        this.waypointGroup = this.shadowRoot.getElementById('waypoint-group');
        this.searchButton = this.shadowRoot.getElementById('search-button');
        this.parisianModeCheckbox = this.shadowRoot.getElementById('parisian-mode');
        
        // Setup event listeners
        this.setupWaypointToggle();
        this.setupSearchButton();
        
        // Original input listener
        if (this.inputElement) {
            this.inputElement.addEventListener("input", (e) => {
                this.input = e.target.value.trim();
                if (this.input === "") {
                    this.clearList();
                    return;
                }
                this.debounce();
            });
        }
        
        document.addEventListener("click", (e) => {
            if (!this.contains(e.target)) this.clearList();
        });
    }
    
    setupWaypointToggle() {
        if (this.addWaypointButton) {
            this.addWaypointButton.addEventListener("click", () => {
                if (this.waypointGroup.style.display === 'none') {
                    this.waypointGroup.style.display = 'flex';
                    this.addWaypointButton.textContent = '- Retirer l\'étape';
                    this.addWaypointButton.style.background = '#ff4444';
                } else {
                    this.waypointGroup.style.display = 'none';
                    this.addWaypointButton.textContent = '+ Ajouter une étape';
                    this.addWaypointButton.style.background = '#4F6FFF';
                    // Clear the input when hiding
                    const waypointInput = this.shadowRoot.getElementById('waypoint');
                    if (waypointInput) waypointInput.value = '';
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

        this.dispatchRouteEvent('search-btn-pressed', "");

        
        const addresses = this.getAllAddresses();
        const parisianMode = this.parisianModeCheckbox.checked;
        
        try {
            await this.fetchRoute(addresses, parisianMode);            
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

    dispatchRouteEvent(eventName, data) {
        const event = new CustomEvent(eventName, {
            detail: data,
            bubbles: true,
            composed: true
        });
        this.dispatchEvent(event);
    }

    async fetchRoute(addresses, parisianMode) {
        const { start, end, waypoint } = addresses;
        
        if (parisianMode) {
            var toSeine = await this.fetchBikeSeine(start);
            this.dispatchRouteEvent('route-display', toSeine);

            //wait 15 seconds
            await new Promise(resolve => setTimeout(resolve, 15000));

            var toEnd = await this.fetchItinerary("Quai de la Seine 75019 Paris", end)
            this.dispatchRouteEvent('route-display', toEnd);
        }
        
        if (!waypoint) {
            var itin =  await this.fetchItinerary(start, end);
            this.dispatchRouteEvent('route-display', itin);
        }
        else{
 
            const firstLeg = await this.fetchItinerary(start, waypoint);
            this.dispatchRouteEvent('route-display',firstLeg);
            
            //wait 15 seconds
            await new Promise(resolve => setTimeout(resolve, 15000));
            
            const secondLeg = await this.fetchItinerary(waypoint, end);
            this.dispatchRouteEvent('route-display', secondLeg);
        }
    }

        async fetchItinerary(start, end) {
            const url = `http://localhost:8701/GPSServer/rest/getItinerary?start=${encodeURIComponent(start)}&end=${encodeURIComponent(end)}`;
            console.log("Calling the API with the URL:\n", url);
            
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error(`Erreur HTTP: ${response.status}`);
            }
            
            return await response.json();
        }

        async fetchBikeSeine(departure) {
            const url = `http://localhost:8701/GPSServer/rest/ThrowBikeSeine?start=${encodeURIComponent(departure)}`;
            console.log("Calling the API with the URL:\n", url);
            
            const response = await fetch(url);
            if (!response.ok) {
                throw new Error(`Erreur HTTP: ${response.status}`);
            }
            
            return await response.json();
        }
    
    getAllAddresses() {
        const start = this.shadowRoot.getElementById('start')?.value || '';
        const end = this.shadowRoot.getElementById('end')?.value || '';
        const waypoint = this.shadowRoot.getElementById('waypoint')?.value || '';
        
        return {
            start: start,
            waypoint: waypoint || null,
            end: end
        };
    }
    
    setAddresses(start, end, waypoint = null) {
        const startInput = this.shadowRoot.getElementById('start');
        const endInput = this.shadowRoot.getElementById('end');
        const waypointInput = this.shadowRoot.getElementById('waypoint');
        const waypointGroup = this.shadowRoot.getElementById('waypoint-group');
        const addButton = this.shadowRoot.getElementById('add-waypoint');
        
        startInput.value = start;
        endInput.value = end;
        
        if (waypoint) {
            // Show waypoint if hidden
            if (waypointGroup.style.display === 'none') {
                addButton.click();
            }
            waypointInput.value = waypoint;
        } else {
            // Hide waypoint if visible
            if (waypointGroup.style.display !== 'none') {
                addButton.click();
            }
        }
    }
    
    clearList() {
        if (this.listElement) {
            this.listElement.innerHTML = '';
        }
    }
    
    debounce() {
        clearTimeout(this.debounceTimer);
        this.debounceTimer = setTimeout(() => {
            this.fetchSuggestions();
        }, 500);
    }
    
    fetchSuggestions() {
        // Your existing implementation
    }
}

customElements.define('route-input', RouteInput);