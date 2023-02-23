using System.Data;
using System.Data.SqlClient;

namespace OrderBooking.Worker
{
    public class AvailableConfirmationMails : IProcessAvailableConfirmationMails
    {
        private string connectionstring;

        public AvailableConfirmationMails(string connectionstring)
        {
            this.connectionstring = connectionstring;
        }

        private readonly string startProcessingSqlCommand = 
@"WITH task AS (
SELECT TOP(1) [dbo].[SalesOrderConfirmations].*, [dbo].[NotificicationPreferences].EmailAddress as BuyerEmailAddress
FROM[dbo].[SalesOrderConfirmations]
        INNER JOIN[dbo].[NotificicationPreferences] on[dbo].[SalesOrderConfirmations].[BuyerId] = [dbo].[NotificicationPreferences].[BuyerId]
        WHERE[dbo].[SalesOrderConfirmations].[Status] = 'Pending')
UPDATE task
SET [Status] = 'Processing'
OUTPUT
    deleted.OrderId,
    deleted.BuyerId,
    deleted.SenderEmailAddress,
    deleted.BuyerEmailAddress,
    deleted.EmailSubject,
    deleted.EmailBody,
    inserted.Status;";

        private readonly string endProcessingSqlCommand = @"UPDATE [dbo].[SalesOrderConfirmations] SET[Status] = @status Where [OrderId] = @orderId;";

        public async Task<ConfirmationMail?> StartProcessing()
        {
            var connection = new SqlConnection(connectionstring);
            connection.Open();

            using var command = new SqlCommand(startProcessingSqlCommand, connection);
            using var dataReader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);

            if (!await dataReader.ReadAsync())
            {
                return null;
            }

            return await ToConfirmationMail(dataReader);
        }

        private async Task<ConfirmationMail> ToConfirmationMail(SqlDataReader dataReader)
        {
            return new ConfirmationMail
            {
                OrderId = await dataReader.GetFieldValueAsync<string>(0),
                BuyerId = await dataReader.GetFieldValueAsync<string>(1),
                SenderEmailAddress = await dataReader.GetFieldValueAsync<string>(2),
                BuyerEmailAddress = await dataReader.GetFieldValueAsync<string>(3),
                EmailSubject = await dataReader.GetFieldValueAsync<string>(4),
                EmailBody = await dataReader.GetFieldValueAsync<string>(5),
                Status = await dataReader.GetFieldValueAsync<string>(6)
            };
        }

        public async Task MarkAsSent(ConfirmationMail mail)
        {
            var connection = new SqlConnection(connectionstring);
            connection.Open();

            using var command = new SqlCommand(endProcessingSqlCommand, connection);
            command.Parameters.AddWithValue("@status", "Sent");
            command.Parameters.AddWithValue("@orderId", mail.OrderId);
            await command.ExecuteNonQueryAsync();
        }

        public async Task MarkAsPending(ConfirmationMail mail)
        {
            var connection = new SqlConnection(connectionstring);
            connection.Open();

            using var command = new SqlCommand(endProcessingSqlCommand, connection);
            command.Parameters.AddWithValue("@status", "Pending");
            command.Parameters.AddWithValue("@orderId", mail.OrderId);
            await command.ExecuteNonQueryAsync();
        }
    }
}
