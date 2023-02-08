namespace OrderBooking.Worker
{
    public static class Postmark
    {
        public static void AddPostmark(this IServiceCollection services)
        {
            services.AddSingleton<ISendEmails>(new PostmarkEmailSender("POSTMARK_API_TEST"));
        }
    }
}
