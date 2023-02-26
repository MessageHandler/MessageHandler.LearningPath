using System.Data;
using System.Data.SqlClient;

namespace OrderBooking.Worker
{
    public class PersistConfirmationMailsToSqlServer : IPersistConfirmationMails
    {
        private string connectionstring;

        public PersistConfirmationMailsToSqlServer(string connectionstring)
        {
            this.connectionstring = connectionstring;
        }

        private readonly string selectSqlCommand = @"SELECT * FROM [dbo].[SalesOrderConfirmations] WHERE [OrderId] = @orderId;";
        private readonly string insertSqlCommand = @"INSERT INTO [dbo].[SalesOrderConfirmations] ([OrderId], [BuyerId], [SenderEmailAddress], [EmailSubject], [EmailBody], [Status]) VALUES (@orderId, @buyerId, @senderEmailAddress, @emailSubject, @emailBody, @status);";
        private readonly string updateSqlCommand = @"UPDATE [dbo].[SalesOrderConfirmations] SET [Status] = @status Where [OrderId] = @orderId;";

        public async Task<ConfirmationMail?> GetConfirmationMail(string id)
        {
            var connection = new SqlConnection(connectionstring);
            connection.Open();

            using var command = new SqlCommand(selectSqlCommand, connection);
            command.Parameters.AddWithValue("@orderId", id);

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
                EmailSubject = await dataReader.GetFieldValueAsync<string>(3),
                EmailBody = await dataReader.GetFieldValueAsync<string>(4),
                Status = await dataReader.GetFieldValueAsync<string>(5)
            };
        }

        public async Task Insert(ConfirmationMail mail)
        {
            var connection = new SqlConnection(connectionstring);
            connection.Open();

            using var command = new SqlCommand(insertSqlCommand, connection);
            command.Parameters.AddWithValue("@orderId", mail.OrderId);
            command.Parameters.AddWithValue("@buyerId", mail.BuyerId);
            command.Parameters.AddWithValue("@senderEmailAddress", mail.SenderEmailAddress);
            command.Parameters.AddWithValue("@emailSubject", mail.EmailSubject);
            command.Parameters.AddWithValue("@emailBody", mail.EmailBody);
            command.Parameters.AddWithValue("@status", "Draft");
            await command.ExecuteNonQueryAsync();
        }

        public async Task Update(ConfirmationMail mail)
        {
            var connection = new SqlConnection(connectionstring);
            connection.Open();

            using var command = new SqlCommand(updateSqlCommand, connection);
            command.Parameters.AddWithValue("@status", mail.Status);
            command.Parameters.AddWithValue("@orderId", mail.OrderId);
            await command.ExecuteNonQueryAsync();
        }
    }
}
