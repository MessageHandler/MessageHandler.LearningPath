namespace OrderBooking.WebAPI.Controllers
{
    public class SetConfirmationMail
    {
        public string BuyerId { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;
    }
}
