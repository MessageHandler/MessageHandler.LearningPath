using Azure.Search.Documents;
using MessageHandler.EventSourcing.Projections;
using MessageHandler.Runtime.AtomicProcessing;
using OrderBooking.Events;

namespace OrderBooking.Worker
{
    public class IndexConfirmedSalesOrder : IHandle<SalesOrderConfirmed>
    {
        private readonly SearchClient _client;
        private readonly IInvokeProjections<SalesOrder> _projection;
        private readonly ILogger<IndexSalesOrder> _logger;

        public IndexConfirmedSalesOrder(IInvokeProjections<SalesOrder> projection, SearchClient client, ILogger<IndexSalesOrder> logger = null!)
        {
            _client = client;
            _projection = projection;
            _logger = logger;
        }

        public async Task Handle(SalesOrderConfirmed message, IHandlerContext context)
        {
            _logger?.LogInformation("Received SalesOrderConfirmed, indexing the sales order...");

            var salesOrder = await _client.GetOrCreateSalesOrderAsync(message.BookingId);

            _projection.Invoke(salesOrder, message);

            await _client.Persist(salesOrder);

            _logger?.LogInformation("Sales order indexed");
        }
    }
}
