
const routeInput = document.querySelector('route-input');
const sideBar = document.querySelector('side-bar');
const map = document.querySelector('map-custom');

sideBar.addEventListener('test-run', (e) => {
    const testId = e.detail.testId;
    switch(testId) {
        case 'test-complex':
            routeInput.setAddresses('Toulouse, France', 'Montpellier, France', 'Lyon');
            break;
            
        case 'test-parisian':
            routeInput.setAddresses('Toulouse, France', 'Lyon, France');
            break;
    }
});

routeInput.addEventListener('route-display', (e) => {
    const geoJson = e.detail;
    
    console.log(geoJson)
    parsed = parseJSONData(geoJson);
    const pedestrian = parsed.pedestrianPath;
    const bike = parsed.bikePath;

    pedestrian.forEach(path => map.loadPath(path, "blue"));
    bike.forEach(path => map.loadPath(path, "green"));


});

async function parseJSONData(data){
    const text = await data.text();
    const geojson = JSON.parse(text)
    return JSON.parse(geojson);
}
