namespace OrderBooking.Worker
{
    public class ConfirmationMail
    {
        public string OrderId { get; set; } = string.Empty;
        public string BuyerId { get; set; } = string.Empty;
        public string SenderEmailAddress { get; set; } = string.Empty;
        public string BuyerEmailAddress { get; set; } = string.Empty;
        public string EmailSubject { get; set; } = string.Empty;
        public string EmailBody { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
