using System.Data;
using System.Data.SqlClient;

namespace OrderBooking.Worker
{
    public class PersistAvailableConfirmationMails : IPersistAvailableConfirmationMails
    {
        private string connectionstring;

        public PersistAvailableConfirmationMails(string connectionstring)
        {
            this.connectionstring = connectionstring;
        }

        private readonly string getAvailableConfirmationMailSqlCommand =
@"WITH task AS (
SELECT TOP(1) [dbo].[SalesOrderConfirmations].*, [dbo].[NotificationPreferences].EmailAddress as BuyerEmailAddress
FROM[dbo].[SalesOrderConfirmations]
        INNER JOIN[dbo].[NotificationPreferences] on[dbo].[SalesOrderConfirmations].[BuyerId] = [dbo].[NotificationPreferences].[BuyerId]
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

        private readonly string updateAvailableConfirmationMailSqlCommand = @"UPDATE [dbo].[SalesOrderConfirmations] SET[Status] = @status Where [OrderId] = @orderId;";

        public async Task<ConfirmationMail?> GetAvailableConfirmationMail()
        {
            var connection = new SqlConnection(connectionstring);
            connection.Open();

            using var command = new SqlCommand(getAvailableConfirmationMailSqlCommand, connection);
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

            using var command = new SqlCommand(updateAvailableConfirmationMailSqlCommand, connection);
            command.Parameters.AddWithValue("@status", "Sent");
            command.Parameters.AddWithValue("@orderId", mail.OrderId);
            await command.ExecuteNonQueryAsync();
        }

        public async Task MarkAsPending(ConfirmationMail mail)
        {
            var connection = new SqlConnection(connectionstring);
            connection.Open();

            using var command = new SqlCommand(updateAvailableConfirmationMailSqlCommand, connection);
            command.Parameters.AddWithValue("@status", "Pending");
            command.Parameters.AddWithValue("@orderId", mail.OrderId);
            await command.ExecuteNonQueryAsync();
        }
    }
}
