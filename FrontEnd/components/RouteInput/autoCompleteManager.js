export class AutocompleteManager {
    constructor(shadowRoot) {
        this.shadowRoot = shadowRoot;
        this.debounceTimers = {
            start: null,
            end: null,
            waypoint: null
        };
    }

    setup(startInput, endInput, waypointInput) {
        this.startInput = startInput;
        this.endInput = endInput;
        this.waypointInput = waypointInput;

        this.startList = this.createSuggestionList('start');
        this.endList = this.createSuggestionList('end');
        this.waypointList = this.createSuggestionList('waypoint');

        this.setupListeners();
        this.setupClickOutside();
    }

    createSuggestionList(inputId) {
        const ul = document.createElement('ul');
        ul.id = `${inputId}-suggestions`;
        ul.className = 'suggestions';
        ul.style.display = 'none';
        
        const input = this.shadowRoot.getElementById(inputId);
        if (input && input.parentElement) {
            input.parentElement.style.position = 'relative';
            input.parentElement.appendChild(ul);
        }
        
        return ul;
    }

    setupListeners() {
        this.startInput.addEventListener("input", (e) => {
            const value = e.target.value.trim();
            if (value === "") {
                this.clearList(this.startList);
                return;
            }
            this.debounceSearch('start', value);
        });

        this.endInput.addEventListener("input", (e) => {
            const value = e.target.value.trim();
            if (value === "") {
                this.clearList(this.endList);
                return;
            }
            this.debounceSearch('end', value);
        });

        this.waypointInput.addEventListener("input", (e) => {
            const value = e.target.value.trim();
            if (value === "") {
                this.clearList(this.waypointList);
                return;
            }
            this.debounceSearch('waypoint', value);
        });
    }

    setupClickOutside() {
        document.addEventListener("click", (e) => {
            if (!this.shadowRoot.host.contains(e.target)) {
                this.clearAllLists();
            }
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
}