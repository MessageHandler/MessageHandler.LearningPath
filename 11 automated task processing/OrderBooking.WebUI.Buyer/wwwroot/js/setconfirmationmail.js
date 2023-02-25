class SetConfirmationEmail extends HTMLElement {

    constructor() {
        super();

        this.innerHTML = `<form>
                            <fieldset>
                                <legend>Enter your email address to opt into confirmation emails</legend>
                                <table>
				                    <tbody>
                                        <tr>
                                            <td><label>Email</label></td>
                                            <td><input id="email" type="email" required /></td>
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

        let buyerId = "buyer1";

        let cmd = {
            buyerId: buyerId,
            emailAddress: event.target.querySelector("#email").value
        }
        let uri = "https://localhost:7100/api/notificationpreferences/" + buyerId;

        let response = await fetch(uri, {
            method: "POST",
            mode: 'cors',
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(cmd)
        });

        if (response.status == 200) {
            window.location = this.redirectUri + "?b=" + buyerId;
        }
        else {
            alert("Unexpected response")
        }
    }

}

customElements.define('set-confirmation-email', SetConfirmationEmail);