namespace OrderBooking.Worker
{
    public class ConfirmationMailWorker : BackgroundService
    {
        private readonly ILogger<ConfirmationMailWorker> logger;
        private readonly SendAvailableConfirmationMails processor;

        public ConfirmationMailWorker(SendAvailableConfirmationMails processor, ILogger<ConfirmationMailWorker> logger = null!)
        {
            this.logger = logger;
            this.processor = processor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await processor.ProcessAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "{Message}", ex.Message);
            }
        }
    }
}
