class PendingOrders extends HTMLElement {

    constructor() {
        super();

        this.innerHTML = `<table>
                            <tbody>
                                <tr>
                                    <th># Widgets</th>
                                    <th>Ordered by</th>
                                    <th>Action</th>
                                </tr>
                            </tbody>
                            <tbody id="orders"></tbody>
                          </table>`;

        this.rowTemplate = `<tr>
                                <td class="text-amount"></td>
                                <td class="text-name"></td>
                                <td><confirm-sales-order></confirm-sales-order></td>
                            </tr>`;
    }

    async connectedCallback() {
        if (!this.salesOrders) {
            this.salesOrders = await this.load();
        }

        this.render();
    }

    async load() {
        let uri = "https://localhost:7100/api/orderbooking/pending/";
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
        var table = this.querySelector("#orders");
        table.innerHTML = "";

        for (var order of this.salesOrders)
        {
            var row = this.htmlToElement(this.rowTemplate);

            var amount = row.querySelector(".text-amount");
            amount.innerHTML = order.amount;

            var name = row.querySelector(".text-name");
            name.innerHTML = order.name;

            var action = row.querySelector("confirm-sales-order");
            action.setAttribute("data-order-id", order.id)

            action.addEventListener("confirmed", () => row.remove());

            table.append(row);
        }
    }

    htmlToElement(html) {
        var template = document.createElement('template');
        html = html.trim(); 
        template.innerHTML = html;
        return template.content.firstChild;
    }
}

customElements.define('pending-orders', PendingOrders);