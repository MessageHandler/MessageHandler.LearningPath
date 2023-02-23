namespace OrderBooking.Worker
{
    public interface IProcessAvailableConfirmationMails 
    {
        Task<ConfirmationMail?> StartProcessing();

        Task MarkAsSent(ConfirmationMail mail);

        Task MarkAsPending(ConfirmationMail mail);
    }
}
