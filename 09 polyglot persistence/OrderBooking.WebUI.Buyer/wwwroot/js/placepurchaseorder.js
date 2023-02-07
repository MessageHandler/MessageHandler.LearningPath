class PlacePurchaseOrder extends HTMLElement {

    constructor() {
        super();

        this.innerHTML = `<form>
                            <fieldset>
                                <legend>How many widgets would you like to buy?</legend>
                                <table>
				                    <tbody>
                                        <tr>
                                            <td><label>Amount</label></td>
                                            <td><input id="amount" type="number" min="1" value="1" required /></td>
                                        </tr>
					                    <tr>
						                    <td colspan="2">
							                    <button type="submit">Submit</button>							                    
						                    </td>
					                    </tr>
				                    </tbody>
			                    </table>
                            </fieldset>
                          </form>`
    }

    static get observedAttributes() {
        return ['data-redirect-uri'];
    }

    get redirectUri() {
        return this.getAttribute('data-redirect-uri');
    }

    set redirectUri(val) {
        if (val) {
            this.setAttribute('data-redirect-uri', val);
        } else {
            this.removeAttribute('data-redirect-uri');
        }
    }

    connectedCallback() {
        var form = this.querySelector("form");
        this.submitBinding = event => this.submitHandler(event);
        form.removeEventListener("submit", this.submitBinding);
        form.addEventListener("submit", this.submitBinding);
    }

    async submitHandler(event)
    {
        event.preventDefault();

        let bookingId = crypto.randomUUID();

        let cmd = {
            bookingId: bookingId,
            name: "Mr. Buyer",
            purchaseOrder: {                
                amount: event.target.querySelector("#amount").valueAsNumber
            }            
        }
        let uri = "https://localhost:7100/api/orderbooking/" + bookingId;

        let response = await fetch(uri, {
            method: "POST",
            mode: 'cors',
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(cmd)
        });

        if (response.status == 200) {
            window.location = this.redirectUri + "?b=" + bookingId;
        }
        else {
            alert("Unexpected response")
        }
    }

}

customElements.define('place-purchase-order', PlacePurchaseOrder);