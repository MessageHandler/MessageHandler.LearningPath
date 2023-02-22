using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace OrderBooking.Worker
{
    public class AutoCreateIndex : IHostedService
    {
        private readonly SearchIndexClient indexClient;

        public AutoCreateIndex(SearchIndexClient indexClient)
        {
            this.indexClient = indexClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var indexName = "salesorders";

            Response<SearchIndex> response = null!;
            try
            {
                response = await indexClient.GetIndexAsync(indexName);
            }   
            catch (Azure.RequestFailedException) { }

            if (response == null || response.Value == null)
            {
                FieldBuilder fieldBuilder = new();
                var searchFields = fieldBuilder.Build(typeof(SalesOrder));

                var index = new SearchIndex(indexName, searchFields);

                await indexClient.CreateOrUpdateIndexAsync(index);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
