class PurchaseOrderNotification extends HTMLElement {

    constructor() {
        super();

        const shadow = this.attachShadow({ mode: "open" });
        

        let style = document.createElement("style");
        style.textContent = `
            .toast {
              visibility: hidden; 
              min-width: 250px; 
              margin-left: -125px; 
              background-color: #333; 
              color: #fff; 
              text-align: center; 
              border-radius: 2px; 
              padding: 16px; 
              position: fixed; 
              z-index: 1; 
              left: 50%; 
              bottom: 30px; 
            }
           
            .toast.show {
              visibility: visible; 
              -webkit-animation: fadein 0.5s, fadeout 0.5s 9.5s;
              animation: fadein 0.5s, fadeout 0.5s 9.5s;
            }
            
            @-webkit-keyframes fadein {
              from {bottom: 0; opacity: 0;}
              to {bottom: 30px; opacity: 1;}
            }

            @keyframes fadein {
              from {bottom: 0; opacity: 0;}
              to {bottom: 30px; opacity: 1;}
            }

            @-webkit-keyframes fadeout {
              from {bottom: 30px; opacity: 1;}
              to {bottom: 0; opacity: 0;}
            }

            @keyframes fadeout {
              from {bottom: 30px; opacity: 1;}
              to {bottom: 0; opacity: 0;}
            }`;
        shadow.appendChild(style);

        this.toast = document.createElement("span");
        this.toast.setAttribute("class", "toast");
        shadow.appendChild(this.toast);
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
        this.toast.innerHTML = "A new purchase order is available for approval.";
        this.toast.classList.add("show");
        setTimeout(() => this.toast.classList.remove("show"), 10000);
    }

}

customElements.define('purchase-order-notification', PurchaseOrderNotification);