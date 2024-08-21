using Azure.Search.Documents;
using MessageHandler.EventSourcing;
using MessageHandler.EventSourcing.AzureTableStorage;
using MessageHandler.EventSourcing.DomainModel;
using MessageHandler.EventSourcing.Outbox;
using MessageHandler.EventSourcing.Projections;
using MessageHandler.Runtime;
using MessageHandler.Runtime.AtomicProcessing;
using NotificationPreferences;
using OrderBooking.Projections;
using OrderBooking.WebAPI;
using OrderBooking.WebAPI.Controllers;
using OrderBooking.WebAPI.SignalR;
using OrderAggregate = OrderBooking.OrderBooking;
using NotificationAggregate = NotificationPreferences.NotificationPreferences;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var searchEndpoint = builder.Configuration.GetValue<string>("SearchEndpoint")
                           ?? throw new Exception("No 'SearchEndpoint' connection string was provided. Use User Secrets or specify via environment variable.");
var apiKey = builder.Configuration.GetValue<string>("SearchApiKey")
             ?? throw new Exception("No 'azuresearchapikey' connection string was provided. Use User Secrets or specify via environment variable.");

builder.Services.AddSearch(searchEndpoint, apiKey);

builder.Services.AddMessageHandler("orderbooking", runtimeConfiguration =>
{
    var storageConnectionString = builder.Configuration.GetValue<string>("TableStorageConnectionString")
                                   ?? throw new Exception("No 'TableStorageConnectionString' was provided. Use User Secrets or specify via environment variable.");

    var serviceBusConnectionString = builder.Configuration.GetValue<string>("ServiceBusConnectionString")
                                   ?? throw new Exception("No 'ServiceBusConnectionString' was provided. Use User Secrets or specify via environment variable.");

    runtimeConfiguration.EventSourcing(source =>
    {
        source.Stream(nameof(OrderBooking.OrderBooking),
            from => from.AzureTableStorage(storageConnectionString, nameof(OrderBooking.OrderBooking)),
            into =>
            {
                into.Aggregate<OrderBooking.OrderBooking>()
                    .EnableTransientChannel<NotifySeller>()
                    .EnableOutbox("OrderBooking", "orderbooking.webapi", pipeline =>
                    {
                        pipeline.RouteMessages(to => to.Topic("orderbooking.events", serviceBusConnectionString));
                    });

                into.Projection<BookingProjection>();
            });
    });

    runtimeConfiguration.EventSourcing(source =>
    {
        source.Stream(nameof(NotificationPreferences.NotificationPreferences),
            from => from.AzureTableStorage(storageConnectionString, nameof(NotificationPreferences.NotificationPreferences)),
            into =>
            {
                into.Aggregate<NotificationPreferences.NotificationPreferences>()
                    .EnableOutbox("NotificationPreferences", "orderbooking.webapi", pipeline =>
                    {
                        pipeline.RouteMessages(to => to.Topic("notificationpreferences.events", serviceBusConnectionString));
                    });
            });
    });
});

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowed(hostName => true);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapHub<EventsHub>("/events");

app.MapPost("api/orderbooking/{id}", 
async (IEventSourcedRepository<OrderAggregate> repo, string id, PlacePurchaseOrder command) =>
{
    var booking = await repo.Get(id);
    booking.PlacePurchaseOrder(command.PurchaseOrder, command.BuyerId, command.Name);

    await repo.Flush();

    return Results.Ok(booking.Id);
});
app.MapGet("api/orderbooking/{id}", async(IRestoreProjections<Booking> projector, string id) =>
    Results.Ok(await projector.Restore(nameof(OrderAggregate), id))
);
app.MapGet("api/orderbooking/pending", async(SearchClient client) =>
{
    var response = await client.SearchAsync<SalesOrder>("*");
    var pendingOrders = response.Value.GetResults().Select(x => x.Document);

    return TypedResults.Ok(pendingOrders);
});
app.MapPost("api/orderbooking/{bookingId}/confirm",
async (IEventSourcedRepository<OrderAggregate> repo, string bookingId) =>
{
    var aggregate = await repo.Get(bookingId);
    aggregate.ConfirmSalesOrder();
    await repo.Flush();

    return Results.Ok(aggregate.Id);
});
app.MapPost("api/notificationpreferences/{buyerId}",
async(IEventSourcedRepository<NotificationAggregate> repo, string buyerId, SetConfirmationMail command) =>
{
    var aggregate = await repo.Get(buyerId);
    aggregate.SetConfirmationEmail(command.EmailAddress);
    await repo.Flush();

    return Results.Ok(aggregate.Id);
});

app.Run();
