

class GeoMap extends HTMLElement {

    constructor() {
        super();
        this.attachShadow({ mode: "open" });
        var map = L.map('map', {
            center: [51.505, -0.09],
            zoom: 13
        });
    }

    

}

customElements.define("geo-map", GeoMap);

