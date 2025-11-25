export class EventDispatcher {
    constructor(element) {
        this.element = element;
    }

    dispatchRouteEvent(eventName, data) {
        const event = new CustomEvent(eventName, {
            detail: data,
            bubbles: true,
            composed: true
        });
        this.element.dispatchEvent(event);
    }

    dispatchMarkerEvent(geoJson, mode) {
        const event = new CustomEvent('load-marker', {
            detail: { geoJson, mode },
            bubbles: true,
            composed: true
        });
        this.element.dispatchEvent(event);
    }
}