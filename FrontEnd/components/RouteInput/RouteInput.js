class RouteInput extends HTMLElement {
    constructor() {
        super();
        this.debounceTimers = {
            start: null,
            end: null,
            waypoint: null
        };
        this.attachShadow({ mode: 'open' });
        this.weather = "";
    }


    setWeather(message){
        this.weather = message;
    }
    
    async connectedCallback() {
        const response = await fetch("./components/RouteInput/RouteInput.html");
        const content = await response.text();
        const templateContent = new DOMParser()
            .parseFromString(content, "text/html")
            .querySelector("template").content;
        this.shadowRoot.appendChild(templateContent.cloneNode(true));
        
        // Get input elements
        this.startInput = this.shadowRoot.getElementById('start');
        this.endInput = this.shadowRoot.getElementById('end');
        this.waypointInput = this.shadowRoot.getElementById('waypoint');
        
        // Get or create suggestion lists
        this.startList = this.shadowRoot.getElementById('start-suggestions') || this.createSuggestionList('start');
        this.endList = this.shadowRoot.getElementById('end-suggestions') || this.createSuggestionList('end');
        this.waypointList = this.shadowRoot.getElementById('waypoint-suggestions') || this.createSuggestionList('waypoint');
        
        // Get other elements
        this.addWaypointButton = this.shadowRoot.getElementById('add-waypoint');
        this.waypointGroup = this.shadowRoot.getElementById('waypoint-group');
        this.searchButton = this.shadowRoot.getElementById('search-button');
        this.parisianModeCheckbox = this.shadowRoot.getElementById('parisian-mode');
        
        // Setup event listeners
        this.setupWaypointToggle();
        this.setupSearchButton();
        this.setupAutocomplete();
        
        // Close suggestions when clicking outside
        document.addEventListener("click", (e) => {
            if (!this.contains(e.target)) {
                this.clearAllLists();
            }
        });
    }
    
    createSuggestionList(inputId) {
        const ul = document.createElement('ul');
        ul.id = `${inputId}-suggestions`;
        ul.className = 'suggestions';
        ul.style.display = 'none';
        
        // Find the input and insert the list after it
        const input = this.shadowRoot.getElementById(inputId);
        if (input && input.parentElement) {
            input.parentElement.style.position = 'relative';
            input.parentElement.appendChild(ul);
        }
        
        return ul;
    }
    
    setupAutocomplete() {
        // Setup autocomplete for start input
        this.startInput.addEventListener("input", (e) => {
            const value = e.target.value.trim();
            if (value === "") {
                this.clearList(this.startList);
                return;
            }
            this.debounceSearch('start', value);
        });
        
        // Setup autocomplete for end input
        this.endInput.addEventListener("input", (e) => {
            const value = e.target.value.trim();
            if (value === "") {
                this.clearList(this.endList);
                return;
            }
            this.debounceSearch('end', value);
        });
        
        // Setup autocomplete for waypoint input
        this.waypointInput.addEventListener("input", (e) => {
            const value = e.target.value.trim();
            if (value === "") {
                this.clearList(this.waypointList);
                return;
            }
            this.debounceSearch('waypoint', value);
        });
    }
    
    debounceSearch(inputType, value) {
        clearTimeout(this.debounceTimers[inputType]);
        this.debounceTimers[inputType] = setTimeout(() => {
            this.fetchSuggestions(inputType, value);
        }, 500);
    }
    
    async fetchSuggestions(inputType, query) {
        try {
            const res = await fetch(`https://api-adresse.data.gouv.fr/search/?q=${encodeURIComponent(query)}&limit=5`);
            const data = await res.json();
            const values = data.features;
            
            let listElement, inputElement;
            switch(inputType) {
                case 'start':
                    listElement = this.startList;
                    inputElement = this.startInput;
                    break;
                case 'end':
                    listElement = this.endList;
                    inputElement = this.endInput;
                    break;
                case 'waypoint':
                    listElement = this.waypointList;
                    inputElement = this.waypointInput;
                    break;
            }
            
            this.renderList(listElement, inputElement, values);
        } catch (err) {
            console.error('Error fetching suggestions:', err);
        }
    }
    
    renderList(listElement, inputElement, values) {
        if (!listElement) return;
        
        listElement.innerHTML = "";
        
        if (values.length === 0) {
            listElement.style.display = 'none';
            return;
        }
        
        values.forEach((element) => {
            const li = document.createElement("li");
            li.textContent = element.properties.label;
            li.className = 'suggestion-item';
            
            li.addEventListener("click", () => {
                inputElement.value = element.properties.label;
                this.clearList(listElement);
            });
            
            listElement.appendChild(li);
        });
        
        listElement.style.display = 'block';
    }
    
    clearList(listElement) {
        if (listElement) {
            listElement.innerHTML = "";
            listElement.style.display = 'none';
        }
    }
    
    clearAllLists() {
        this.clearList(this.startList);
        this.clearList(this.endList);
        this.clearList(this.waypointList);
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
                    this.clearList(this.waypointList);
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

        if(this.weather === "cloud"){
            alert("Attention : conditions météorologiques difficiles (pluie). Soyez prudent lors de votre trajet !");
            if(!waypoint){
                var walk = await this.fetchWalk(start, end);
                this.dispatchRouteEvent('route-display', walk);
            }
            else{
                const firstLeg = await this.fetchWalk(start, waypoint);
                this.dispatchRouteEvent('route-display', firstLeg);
                const secondLeg = await this.fetchWalk(waypoint, end);
                this.dispatchRouteEvent('route-display', secondLeg);

            }
        }
        else{
            
            if (parisianMode) {
                var toSeine = await this.fetchBikeSeine(start);
                this.dispatchRouteEvent('route-display', toSeine);


                var toEnd = await this.fetchItinerary("Quai de la Seine 75019 Paris", end)
                this.dispatchRouteEvent('route-display', toEnd);
            }
            
            else{
                if (!waypoint) {
                    var itin = await this.fetchItinerary(start, end);
                    this.dispatchRouteEvent('route-display', itin);
                } else {
                    const firstLeg = await this.fetchItinerary(start, waypoint);
                    this.dispatchRouteEvent('route-display', firstLeg);
                                    
                    const secondLeg = await this.fetchItinerary(waypoint, end);
                    this.dispatchRouteEvent('route-display', secondLeg);
                }
            }
        }
    }

    async fetchWalk(start, end) {
        const url = `http://localhost:8701/GPSServer/rest/getWalk?start=${encodeURIComponent(start)}&end=${encodeURIComponent(end)}`;
        console.log("Calling the API with the URL:\n", url);
        
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Erreur HTTP: ${response.status}`);
        }
        return await response.json();
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
        const start = this.startInput?.value || '';
        const end = this.endInput?.value || '';
        const waypoint = this.waypointInput?.value || '';
        
        return {
            start: start,
            waypoint: waypoint || null,
            end: end
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

    setParisianMode(val){
        this.parisianModeCheckbox.checked = val;

    }

}

customElements.define('route-input', RouteInput);