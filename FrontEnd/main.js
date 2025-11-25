
const routeInput = document.querySelector('route-input');
const sideBar = document.querySelector('side-bar');
const map = document.querySelector('map-custom');
const weatherMQ = document.querySelector('weather-mq');

sideBar.addEventListener('test-run', (e) => {
    const testId = e.detail.testId;
    switch(testId) {
        case 'test-complex':
            routeInput.setAddresses( '2 Passage des Antonins 69100 Villeurbanne', '2 Rue Job 31000 Toulouse', '4 Rue d’Emporion 34970 Lattes');
            routeInput.setParisianMode(false);
            routeInput.setPilgrimMode(false);
            
            break;
            
        case 'test-parisian':
            routeInput.setAddresses('2 Rue Job 31000 Toulouse', '3 rue des orangers 34970 lattes');
            routeInput.setParisianMode(true);
            routeInput.setPilgrimMode(false);

            break;
        
        case 'test-basic':
            routeInput.setAddresses('2 Rue Job 31000 Toulouse', "Rue du Rouergue 31100 Toulouse")
            routeInput.setParisianMode(false);
            routeInput.setPilgrimMode(false);

            break;   
        case 'test-pel':
            routeInput.setAddresses('2 Rue Judaïque 33000 Bordeaux', '22 Boulevard georges guynemer 13009 Marseille');
            routeInput.setParisianMode(false);
            routeInput.setPilgrimMode(true);
            break;
    }
});

weatherMQ.addEventListener('eventMQ', (e) => {
    console.log("Nouveau message météo reçu dans main.js :", e.detail.body);
    routeInput.setEvent(e.detail.body);
});



routeInput.addEventListener('search-btn-pressed', (e) =>{
    map.clearPath();
})

routeInput.addEventListener('route-display', async (e) => {
    // Attendre que le custom element soit défini
    await customElements.whenDefined('map-custom');
    
    let geoJson = e.detail;
    // Si c'est une string, on la parse
    if (typeof geoJson === "string") {
        try {
            geoJson = JSON.parse(geoJson);
        } catch(err) {
            console.error("Erreur JSON.parse:", err, geoJson);
            return;
        }
    }
    
    const pedestrian = geoJson.pedestrianPath;
    const bike = geoJson.bikePath;
    
    console.log(pedestrian);
    console.log(bike);

    pedestrian.forEach(path => {
            map.loadPath(path, "blue");
        });
    
    bike.forEach(path => {
        map.loadPath(path, "green");
    });
    

});

routeInput.addEventListener('load-marker', (e) => {
    let geoJson = e.detail.geoJson;
    let mode = e.detail.mode;
    
    // Si c'est une string, on la parse
    if (typeof geoJson === "string") {
        try {
            geoJson = JSON.parse(geoJson);
        } catch(err) {
            console.error("Erreur JSON.parse:", err, geoJson);
            return;
        }
    }
    
    console.log(mode)

    if(mode === "departure"){
        let point = getPosCoordinates(geoJson.pedestrianPath[0], "departure");
        map.addMarker(point, "Départ", "#22c55e");
    } 
    else if(mode === "arrival" && !routeInput.istPilgrimModeActived()){
        console.log("oooh")
        let point = getPosCoordinates(geoJson.pedestrianPath[geoJson.pedestrianPath.length - 1], "arrival");
        map.addMarker(point, "Arrivée", "#ef4444");
    } 
    else if(mode === "arrival" && routeInput.istPilgrimModeActived()){
        let point = getPosCoordinates(geoJson.bikePath[geoJson.bikePath.length - 1], "arrival");
        map.addMarker(point, "Arrivée", "#ef4444");
    }
});


function getPosCoordinates(geoJsonPath, mode) {
    if (!geoJsonPath || !geoJsonPath.features) return null;
        
    const coords = geoJsonPath.features[0]?.geometry?.coordinates;
    if (!coords || coords.length === 0) return null;
        
    if(mode === "departure") {
        return coords[0];
    } 
    else if(mode === "arrival"){
        return coords[coords.length - 1]
    }
}
