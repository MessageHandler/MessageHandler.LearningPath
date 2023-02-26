namespace OrderBooking.Worker
{
    public interface IPersistConfirmationMails
    {
        Task<ConfirmationMail?> GetConfirmationMail(string id);

        Task Insert(ConfirmationMail mail);

        Task Update(ConfirmationMail mail);
    }
}
