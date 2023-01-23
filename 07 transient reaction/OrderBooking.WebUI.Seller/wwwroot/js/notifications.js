class PurchaseOrderNotification extends HTMLElement {

    constructor() {
        super();       
    }

    async connectedCallback() {

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:7100/events")
            .configureLogging(signalR.LogLevel.Information)
            .build();        

        this.connection.onclose(async () => {
            await this.start();
        });
        await this.start();

        this.connection.on("Notify", (event) => {
            this.render();
        });

        await this.connection.invoke("Subscribe");
    }

    async start() {
        try {
            await this.connection.start();
        } catch (err) {
            console.log(err);
        }
    };

    render() {
        this.innerHTML = "<h1>A new purchase order is available for approval.</h1>";
    }

}

customElements.define('purchase-order-notification', PurchaseOrderNotification);