using MessageHandler.EventSourcing;
using MessageHandler.EventSourcing.AzureTableStorage;
using MessageHandler.EventSourcing.Outbox;
using MessageHandler.Runtime;
using MessageHandler.Runtime.AtomicProcessing;
using OrderBooking.Projections;
using OrderBooking.WebAPI;
using OrderBooking.WebAPI.SignalR;

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

app.MapControllers();

app.MapHub<EventsHub>("/events");

app.Run();
