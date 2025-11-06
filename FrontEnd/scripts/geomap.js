let map; // rendre la carte globale
let routesLayer; // contiendra les tracés d'itinéraires


function loadMap() {
    map = L.map('map').setView([48.8566, 2.3522], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap Contributors'
    }).addTo(map);

    routesLayer = L.layerGroup().addTo(map);

}

async function findPath() {
    try {
        routesLayer.clearLayers();

        const departureComponent = document.getElementById("departure");
        const arrivalComponent = document.getElementById("arrival");

        const departureInput = departureComponent.shadowRoot.querySelector("input");
        const arrivalInput = arrivalComponent.shadowRoot.querySelector("input");

        if (!departureInput || !arrivalInput) {
            console.error("Les champs departure ou arrival sont introuvables !");
            return;
        }

        const departure = encodeURIComponent(departureInput.value.trim());
        const arrival = encodeURIComponent(arrivalInput.value.trim());

        if (!departure || !arrival) {
            console.warn("Veuillez remplir les deux champs.");
            return;
        }


        const url = `http://localhost:8701/GPSServer/getItinerary?start=${departure}&end=${arrival}`;

        console.log("Calling the API with the URL : \n", url)

        const response = await fetch(url);

        if (!response.ok) {
            throw new Error(`Erreur HTTP: ${response.status}`);
        }

        const text = await response.text();      // ← récupère la chaîne brute
        const geojson = JSON.parse(text);        // ← parse le premier JSON
        const parsed = JSON.parse(geojson);      // ← parse le deuxième niveau (celui qui contient pedestrianPath)


        const pedestrian = parsed.pedestrianPath;
        const bike = parsed.bikePath;


        // Affichage des chemins

        routesLayer.clearLayers();
        pedestrian.forEach(path => loadPath(path, "blue"));
        bike.forEach(path => loadPath(path, "green"));
    } catch (err) {
        console.error("Erreur lors de la récupération de l’itinéraire :", err);
    }
}

function loadPath(data, color) {
    const itineraire = L.geoJSON(data, {
      style: {
        color: color,
        weight: 4
      }
    }).addTo(routesLayer);

    map.fitBounds(itineraire.getBounds());
}

loadMap();
