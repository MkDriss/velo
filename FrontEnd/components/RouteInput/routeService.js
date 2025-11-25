export class RouteService {
    constructor(baseUrl = 'http://localhost:8701/GPSServer/rest') {
        this.baseUrl = baseUrl;
    }

    async fetchBike(start, end) {
        const url = `${this.baseUrl}/getBike?start=${encodeURIComponent(start)}&end=${encodeURIComponent(end)}`;
        console.log("Calling the API with the URL:\n", url);
        
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Erreur HTTP: ${response.status}`);
        }
        return await response.json();
    }

    async fetchWalk(start, end) {
        const url = `${this.baseUrl}/getWalk?start=${encodeURIComponent(start)}&end=${encodeURIComponent(end)}`;
        console.log("Calling the API with the URL:\n", url);
        
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Erreur HTTP: ${response.status}`);
        }
        return await response.json();
    }

    async fetchItinerary(start, end) {
        const url = `${this.baseUrl}/getItinerary?start=${encodeURIComponent(start)}&end=${encodeURIComponent(end)}`;
        console.log("Calling the API with the URL:\n", url);
        
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Erreur HTTP: ${response.status}`);
        }
        
        return await response.json();
    }

    async fetchBikeSeine(departure) {
        const url = `${this.baseUrl}/ThrowBikeSeine?start=${encodeURIComponent(departure)}`;
        console.log("Calling the API with the URL:\n", url);
        
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Erreur HTTP: ${response.status}`);
        }
        
        return await response.json();
    }

    async fetchPilgrimRoute() {
        const url = `${this.baseUrl}/getPelerin`;
        console.log("Calling the API with the URL:\n", url);
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Erreur HTTP: ${response.status}`);
        }
        return await response.json();
    }
}