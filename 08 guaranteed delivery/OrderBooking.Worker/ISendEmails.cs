namespace OrderBooking.Worker
{
    public interface ISendEmails
    {
        Task SendAsync(string from, string to, string subject, string body);
    }
}
