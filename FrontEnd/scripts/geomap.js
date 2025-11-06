function loadMap() {
    const map = L.map('map').setView([48.8566, 2.3522], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap Contributors'
    }).addTo(map);
}

async function findPath() {
    try {
        const departureComponent = document.getElementById("departure");
        const arrivalComponent = document.getElementById("arrival");

        // Accéder aux inputs à l’intérieur du shadowRoot
        const departureInput = departureComponent.shadowRoot.querySelector("input");
        const arrivalInput = arrivalComponent.shadowRoot.querySelector("input");

        if (!departureInput || !arrivalInput) {
            console.error("Les champs departure ou arrival sont introuvables !");
            return;
        }

        const departure = encodeURIComponent(departureInput.value.trim());
        console.log("Departure set :", departure);
        const arrival = encodeURIComponent(arrivalInput.value.trim());
        console.log("Arrival set :", arrival);

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

        const geojson = await response.json();

        console.log("GeoJSON reçu :", geojson);



        // Optionnel : tu pourras ensuite l’ajouter sur ta carte Leaflet
        // L.geoJSON(geojson).addTo(map);

    } catch (err) {
        console.error("Erreur lors de la récupération de l’itinéraire :", err);
    }
}


function loadPath() { }

loadMap();

// // Chargement du GeoJSON
// fetch("itineraire.geojson")
//   .then(response => response.json())
//   .then(data => {
//     const itineraire = L.geoJSON(data, {
//       style: {
//         color: 'red',
//         weight: 4
//       },
//       onEachFeature: (feature, layer) => {
//         if (feature.properties && feature.properties.name) {
//           layer.bindPopup(feature.properties.name);
//         }
//       }
//     }).addTo(map);

//     // Zoom automatique sur l'itinéraire
//     map.fitBounds(itineraire.getBounds());
//   });

