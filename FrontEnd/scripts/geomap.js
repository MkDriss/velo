let map; // ← rendre la carte globale

function loadMap() {
    map = L.map('map').setView([48.8566, 2.3522], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap Contributors'
    }).addTo(map);
}

async function findPath() {
    try {
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

        // utilisation de la réponse mockée
        const response = JSON.parse('{\"pedestrianPath\":[{\"type\":\"FeatureCollection\",\"bbox\":[1.467649,43.584044,1.46773,43.584077],\"features\":[{\"bbox\":[1.467649,43.584044,1.46773,43.584077],\"type\":\"Feature\",\"properties\":{\"segments\":[{\"distance\":7.5,\"duration\":5.4,\"steps\":[{\"distance\":7.5,\"duration\":5.4,\"type\":11,\"instruction\":\"Head northwest on Rue Ferrando\",\"name\":\"Rue Ferrando\",\"way_points\":[0,1]},{\"distance\":0.0,\"duration\":0.0,\"type\":10,\"instruction\":\"Arrive at Rue Ferrando, on the right\",\"name\":\"-\",\"way_points\":[1,1]}]}],\"way_points\":[0,1],\"summary\":{\"distance\":7.5,\"duration\":5.4}},\"geometry\":{\"coordinates\":[[1.46773,43.584044],[1.467649,43.584077]],\"type\":\"LineString\"}}],\"metadata\":{\"attribution\":\"openrouteservice.org | OpenStreetMap contributors\",\"service\":\"routing\",\"timestamp\":1762429278478,\"query\":{\"coordinates\":[[1.4677301,43.5840438],[1.4677552,43.5842119]],\"profile\":\"foot-walking\",\"profileName\":\"foot-walking\",\"format\":\"geojson\"},\"engine\":{\"version\":\"9.3.0\",\"build_date\":\"2025-06-06T15:39:25Z\",\"graph_date\":\"2025-10-30T00:32:48Z\",\"osm_date\":\"2025-10-20T00:00:01Z\"}}},{\"type\":\"FeatureCollection\",\"bbox\":[1.467649,43.584044,1.46773,43.584077],\"features\":[{\"bbox\":[1.467649,43.584044,1.46773,43.584077],\"type\":\"Feature\",\"properties\":{\"segments\":[{\"distance\":7.5,\"duration\":5.4,\"steps\":[{\"distance\":7.5,\"duration\":5.4,\"type\":11,\"instruction\":\"Head northwest on Rue Ferrando\",\"name\":\"Rue Ferrando\",\"way_points\":[0,1]},{\"distance\":0.0,\"duration\":0.0,\"type\":10,\"instruction\":\"Arrive at Rue Ferrando, on the right\",\"name\":\"-\",\"way_points\":[1,1]}]}],\"way_points\":[0,1],\"summary\":{\"distance\":7.5,\"duration\":5.4}},\"geometry\":{\"coordinates\":[[1.46773,43.584044],[1.467649,43.584077]],\"type\":\"LineString\"}}],\"metadata\":{\"attribution\":\"openrouteservice.org | OpenStreetMap contributors\",\"service\":\"routing\",\"timestamp\":1762429278632,\"query\":{\"coordinates\":[[1.4677301,43.5840438],[1.4677552,43.5842119]],\"profile\":\"foot-walking\",\"profileName\":\"foot-walking\",\"format\":\"geojson\"},\"engine\":{\"version\":\"9.3.0\",\"build_date\":\"2025-06-06T15:39:25Z\",\"graph_date\":\"2025-10-30T00:32:48Z\",\"osm_date\":\"2025-10-20T00:00:01Z\"}}}],\"bikePath\":[{\"type\":\"FeatureCollection\",\"bbox\":[1.467649,43.584044,1.46773,43.584077],\"features\":[{\"bbox\":[1.467649,43.584044,1.46773,43.584077],\"type\":\"Feature\",\"properties\":{\"segments\":[{\"distance\":7.5,\"duration\":1.5,\"steps\":[{\"distance\":7.5,\"duration\":1.5,\"type\":11,\"instruction\":\"Head northwest on Rue Ferrando\",\"name\":\"Rue Ferrando\",\"way_points\":[0,1]},{\"distance\":0.0,\"duration\":0.0,\"type\":10,\"instruction\":\"Arrive at Rue Ferrando, on the right\",\"name\":\"-\",\"way_points\":[1,1]}]}],\"way_points\":[0,1],\"summary\":{\"distance\":7.5,\"duration\":1.5}},\"geometry\":{\"coordinates\":[[1.46773,43.584044],[1.467649,43.584077]],\"type\":\"LineString\"}}],\"metadata\":{\"attribution\":\"openrouteservice.org | OpenStreetMap contributors\",\"service\":\"routing\",\"timestamp\":1762429278556,\"query\":{\"coordinates\":[[1.4677301,43.5840438],[1.4677552,43.5842119]],\"profile\":\"cycling-regular\",\"profileName\":\"cycling-regular\",\"format\":\"geojson\"},\"engine\":{\"version\":\"9.3.0\",\"build_date\":\"2025-06-06T15:39:25Z\",\"graph_date\":\"2025-10-31T03:00:42Z\",\"osm_date\":\"2025-10-20T00:00:01Z\"}}}]}')

        const pedestrian = response.pedestrianPath;
        const bike = response.bikePath; // ← correction

        // Affichage des chemins
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
    }).addTo(map);

    map.fitBounds(itineraire.getBounds());
}

loadMap();
