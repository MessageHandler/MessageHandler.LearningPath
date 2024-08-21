using Azure.Data.Tables;
using MessageHandler.EventSourcing.AzureTableStorage;
using MessageHandler.EventSourcing.Contracts;
using Microsoft.Extensions.Configuration;
using OrderBooking.Events;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OrderBooking.IntegrationTests
{
    public class WhileIntegratingTableStorage : IAsyncLifetime
    {
        private string connectionString;
        private string tableName = "t" + Guid.NewGuid().ToString("N");

        public WhileIntegratingTableStorage()
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<WhileIntegratingTableStorage>(optional: true)
                .AddEnvironmentVariables()                
                .Build();

            connectionString = configuration["TableStorageConnectionString"];                        
        }

        [Fact]
        public async Task GivenAnEventStream_WhenPersistingEvents_ThenCanLoadEvents()
        {
            var streamId = "fe8430bf-00a4-42b7-b077-87d8fff4ba68";
            var streamType = "OrderBooking";

            var started = new BookingStarted
            {
                EventId = "89795ced-ea64-46c2-879e-10d285a09429", // unique
                SourceId = streamId,
                Version = 1,
                PurchaseOrder = new PurchaseOrder()
            };
            var confirmed = new SalesOrderConfirmed
            {
                EventId = "9a5937c2-5e14-461f-b452-fa504f300d15", // unique
                SourceId = streamId,
                Version = 2,
                TargetBranchParentId = started.EventId
            };

            SourcedEvent[] eventStream = [ confirmed, started ]; // will be reordered

            var eventSource = new AzureTableStorageEventSource(connectionString, tableName);

            await eventSource.Persist(streamType, streamId, eventStream);
            var history = await eventSource.Load(streamType, streamId, version: 0);

            Assert.Equal(2, history.Count());

            Assert.Equal(started.EventId, history.First().EventId);
            Assert.Equal(confirmed.EventId, history.Last().EventId);

            Assert.IsType<BookingStarted>(history.First());
            Assert.IsType<SalesOrderConfirmed>(history.Last());
        }

        public async Task InitializeAsync()
        {
            var client = new TableServiceClient(connectionString);

            var table = client.GetTableClient(tableName);

            await table.CreateIfNotExistsAsync();
        }

        public async Task DisposeAsync()
        {
            try
            {
                var client = new TableServiceClient(connectionString);

                var table = client.GetTableClient(tableName);

                await table.DeleteAsync();
            }
            catch { }
        }
    }
}