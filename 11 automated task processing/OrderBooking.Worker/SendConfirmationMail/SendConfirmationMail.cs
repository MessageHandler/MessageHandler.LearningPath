namespace OrderBooking.Worker
{
    public class SendConfirmationMail : BackgroundService
    {
        private readonly ILogger<SendConfirmationMail> logger;
        private readonly ISendEmails emailSender;
        private readonly IProcessAvailableConfirmationMails processor;

        public SendConfirmationMail(IProcessAvailableConfirmationMails processor, ISendEmails emailSender, ILogger<SendConfirmationMail> logger = null!) {
            this.logger = logger;
            this.emailSender = emailSender;
            this.processor = processor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var email = await processor.StartProcessing();

                    if(email != null)
                    {
                        try
                        {
                            logger?.LogInformation("Sending confirmation mail...");

                            await emailSender.SendAsync(email.SenderEmailAddress,
                                                        email.BuyerEmailAddress,
                                                        email.EmailSubject,
                                                        email.EmailBody);

                            await this.processor.MarkAsSent(email);

                            logger?.LogInformation("Confirmation mail marked as sent...");
                        }
                        catch (Exception)
                        {
                            await this.processor.MarkAsPending(email);

                            logger?.LogInformation("Sending confirmation mail failed, marked as pending...");
                        }                        
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                    }                    
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "{Message}", ex.Message);
            }
        }
    }
}
