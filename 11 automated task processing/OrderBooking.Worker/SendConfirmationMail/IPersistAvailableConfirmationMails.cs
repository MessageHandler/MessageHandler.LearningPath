namespace OrderBooking.Worker
{
    public interface IPersistAvailableConfirmationMails 
    {
        Task<ConfirmationMail?> GetAvailableConfirmationMail(); //GetAvailableConfirmationMail

        Task MarkAsSent(ConfirmationMail mail);

        Task MarkAsPending(ConfirmationMail mail);
    }
}
