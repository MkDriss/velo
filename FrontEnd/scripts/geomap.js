let map; // rendre la carte globale
let routesLayer; // contiendra les tracés d'itinéraires


function loadMap() {
    map = L.map('map').setView([48.8566, 2.3522], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap Contributors'
    }).addTo(map);

    routesLayer = L.layerGroup().addTo(map);
}

function getRoutePoints() {
    // Sélectionner tous les input-custom dans l'ordre d'affichage
    const inputs = document.querySelectorAll(".input-section input-custom");

    const points = [];

    inputs.forEach(inputCustom => {
        const input = inputCustom.shadowRoot.querySelector("input");

        if (input) {
            const value = input.value.trim();

            if (value && value !== "") {
                points.push(encodeURIComponent(value));
            }
        }
    });

    // Vérification : au minimum un départ + une arrivée
    if (points.length < 2) {
        console.warn("Veuillez remplir au moins Departure et Arrival.");
        return null;
    }

    return points;
}

async function fetchBikeSeine(departure){
    const url = `http://localhost:8701/GPSServer/rest/ThrowBikeSeine?start=${departure}`;
    console.log("Calling the API with the URL : \n", url)

    const response = await fetch(url);

    if (!response.ok) {
        throw new Error(`Erreur HTTP: ${response.status}`);
    }

    return response;
}

async function fetchData(departure, arrival){
    const url = `http://localhost:8701/GPSServer/rest/getItinerary?start=${departure}&end=${arrival}`;

    console.log("Calling the API with the URL : \n", url)

    console.log("fetching data | departure :", departure, " arrival :", arrival)

    const response = await fetch(url);

    if (!response.ok) {
        throw new Error(`Erreur HTTP: ${response.status}`);
    }

    return response;
}

async function displayRoute(departure, arrival){
        console.log("Calcul d'itinéraire en cours");
        const response = await fetchData(departure, arrival);
        console.log("Server Response : ", response)
        const parsed = await parseJSONData(response);     

        const pedestrian = parsed.pedestrianPath;
        const bike = parsed.bikePath;

        pedestrian.forEach(path => loadPath(path, "blue"));
        bike.forEach(path => loadPath(path, "green"));
}


async function displayParisian(departure, arrival){
        console.log("Calcul d'itinéraire parisien en cours");
        const response = await fetchBikeSeine(departure);
        const parsed = await parseJSONData(response);   
        const pedestrian = parsed.pedestrianPath;
        const bike = parsed.bikePath;
        
        pedestrian.forEach(path => loadPath(path, "blue"));
        bike.forEach(path => loadPath(path, "green"));

        await sleep(60000);


        const reponse2 = await fetchData("Quai de la Seine 75019 Paris", arrival);

        
        const parsed2 = await parseJSONData(reponse2);   

        const pedestrian2 = parsed2.pedestrianPath;
        const bike2 = parsed2.bikePath;

        pedestrian2.forEach(path => loadPath(path, "blue"));
        bike2.forEach(path => loadPath(path, "green"));
}

async function parseJSONData(data){
    const text = await data.text();
    const geojson = JSON.parse(text)
    return JSON.parse(geojson);
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

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function findPath(){
    try {
        routesLayer.clearLayers();

        const points = getRoutePoints();

        console.log(points);

        for(let i = 0; i < points.length - 1; i++){
            await displayRoute(points[i], points[i+1]);
            if (i < points.length - 2) {
                console.log("Attente 30 secondes avant le prochain itinéraire...");
                await sleep(60000);
            }
            console.log("Trajets calculés")
        }

    } catch (err) {
        console.error("Erreur lors de la récupération de l’itinéraire :", err);
    }
}

async function findParisianPath(){

    try {
        routesLayer.clearLayers();
        var start = getInputComponent("departure").value;
        var end = getInputComponent("arrival").value;
        await displayParisian(start, end);
            

    } catch (err) {
        console.error("Erreur lors de la récupération de l’itinéraire :", err);
    }

}

function getInputComponent(id){
    return document.getElementById(id).shadowRoot.querySelector("input");
}

loadMap();
