namespace OrderBooking.Worker
{
    public class ConfirmationMail
    {
        public string OrderId { get; set; }
        public string BuyerId { get; set; }
        public string SenderEmailAddress { get; set; }
        public string BuyerEmailAddress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string Status { get; set; }
    }
}
