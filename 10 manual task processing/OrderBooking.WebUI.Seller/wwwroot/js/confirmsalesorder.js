class ConfirmSalesOrder extends HTMLElement {

    constructor() {
        super();

        this.innerHTML = `<button>Confirm</button>`;
    }

    static get observedAttributes() {
        return ['data-booking-id'];
    }

    get orderId() {
        return this.getAttribute('data-order-id');
    }

    set orderId(val) {
        if (val) {
            this.setAttribute('data-order-id', val);
        } else {
            this.removeAttribute('data-order-id');
        }
    }

    async connectedCallback() {
        var button = this.querySelector("button");
        this.confirmBinding = () => this.confirm();
        button.removeEventListener("click", this.confirmBinding);
        button.addEventListener("click", this.confirmBinding);
    }

    async confirm() {
        let cmd = {
            bookingId: this.orderId
        }
        let uri = "https://localhost:7100/api/orderbooking/" + this.orderId + "/confirm/";
        let response = await fetch(uri, {
            method: "PUT",
            mode: 'cors',
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(cmd)
        });

        if (response.status == 200) {
            this.dispatchEvent(new Event("confirmed"));
        }
    }

}

customElements.define('confirm-sales-order', ConfirmSalesOrder);