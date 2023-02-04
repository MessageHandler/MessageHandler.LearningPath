class BookingState extends HTMLElement {

    constructor() {
        super();

        this.params = new Proxy(new URLSearchParams(window.location.search), {
            get: (searchParams, prop) => searchParams.get(prop),
        });
    }

    get bookingId() {        
        return this.params.b;
    }

    async connectedCallback() {
        if (!this.booking)
        {
            this.booking = await this.load();
        }

        this.render();
    }

    async load() {
        let uri = "https://localhost:7100/api/orderbooking/" + this.bookingId;
        let response = await fetch(uri, {
            method: "GET",
            mode: 'cors',
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (response.status == 200) {
            return await response.json();
        }
        else {
            return null;
        }
    }

    render() {
        this.innerHTML = this.booking?.status;
    }

}

customElements.define('booking-state', BookingState);