namespace OrderBooking.Worker
{
    public class SendAvailableConfirmationMails
    {
        private readonly ILogger<SendAvailableConfirmationMails> logger;
        private readonly ISendEmails emailSender;
        private readonly IPersistAvailableConfirmationMails storage;

        public SendAvailableConfirmationMails(IPersistAvailableConfirmationMails storage, ISendEmails emailSender, ILogger<SendAvailableConfirmationMails> logger = null!)
        {
            this.logger = logger;
            this.emailSender = emailSender;
            this.storage = storage;
        }

        public async Task ProcessAsync(CancellationToken stoppingToken)
        {
            var email = await storage.GetAvailableConfirmationMail();

            if (email != null)
            {
                try
                {
                    logger?.LogInformation("Confirmation mail available, sending it...");

                    await emailSender.SendAsync(email.SenderEmailAddress,
                                                         email.BuyerEmailAddress,
                                                         email.EmailSubject,
                                                         email.EmailBody);

                    await this.storage.MarkAsSent(email);

                    logger?.LogInformation("Confirmation mail marked as sent...");
                }
                catch (Exception)
                {
                    await this.storage.MarkAsPending(email);

                    logger?.LogInformation("Sending confirmation mail failed, marked as pending...");
                }
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
