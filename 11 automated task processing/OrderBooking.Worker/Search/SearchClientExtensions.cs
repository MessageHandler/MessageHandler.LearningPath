using Azure;
using Azure.Search.Documents;

namespace OrderBooking.Worker
{
    public static class SearchClientExtensions
    {
        public static async Task<SalesOrder> GetOrCreateSalesOrderAsync(this SearchClient client, string orderId)
        {
            var order = default(SalesOrder);
            try
            {
                order = await client.GetDocumentAsync<SalesOrder>(orderId);
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status == 404)
                {
                    order = new SalesOrder() { Id = orderId };
                }
            }
            return order!;
        }

        public static async Task Persist(this SearchClient client, SalesOrder salesOrder)
        {
            await client.MergeOrUploadDocumentsAsync(new[] { salesOrder });
        }
    }
}
