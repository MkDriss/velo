
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
            break;
            
        case 'test-parisian':
            routeInput.setAddresses('2 Rue Job 31000 Toulouse', '3 rue des orangers 34970 lattes');
            routeInput.setParisianMode(true);

            break;
        
        case 'test-basic':
            routeInput.setAddresses('2 Rue Job 31000 Toulouse', "Rue du Rouergue 31100 Toulouse")
            routeInput.setParisianMode(false);

            break;   
        case 'test-pel':
            routeInput.setAddresses('2 Rue Judaïque 33000 Bordeaux', '22 Boulevard georges guynemer 13009 Marseille');     
    }
});

weatherMQ.addEventListener('eventMQ', (e) => {
    console.log("Nouveau message météo reçu dans main.js :", e.detail.body);
    routeInput.setEvent(e.detail.body);
});



routeInput.addEventListener('search-btn-pressed', (e) =>{
    map.clearPath();
})





routeInput.addEventListener('route-display', (e) => {
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

    console.log(pedestrian)
    console.log(bike)

    if (pedestrian) pedestrian.forEach(path => map.loadPath(path, "blue"));
    if (bike) bike.forEach(path => map.loadPath(path, "green"));
});
