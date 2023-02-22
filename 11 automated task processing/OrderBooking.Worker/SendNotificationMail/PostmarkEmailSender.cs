using PostmarkDotNet;

namespace OrderBooking.Worker
{
    public class PostmarkEmailSender : ISendEmails
    {
        private string serverToken;

        public PostmarkEmailSender(string serverToken)
        {
            this.serverToken = serverToken;
        }

        public async Task SendAsync(string from, string to, string subject, string body)
        {
            var email = new PostmarkMessage()
            {
                From = from,
                To = to,
                Subject = subject,
                HtmlBody = body,
                TextBody = body
            };

            var client = new PostmarkClient(this.serverToken);
            await client.SendMessageAsync(email);
        }
    }
}
