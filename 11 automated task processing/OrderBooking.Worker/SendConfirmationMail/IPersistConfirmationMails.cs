namespace OrderBooking.Worker
{
    public interface IPersistConfirmationMails
    {
        Task<ConfirmationMail?> GetConfirmationMail(string id);

        Task Persist(ConfirmationMail mail);

        Task MarkAsPending(ConfirmationMail mail);
    }
}
