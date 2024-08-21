using MessageHandler.EventSourcing;
using MessageHandler.EventSourcing.AzureTableStorage;
using MessageHandler.Runtime;
using OrderBooking.Projections;
using OrderBooking.WebAPI;
using OrderBooking.WebAPI.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMessageHandler("orderbooking", runtimeConfiguration =>
{
    var connectionString = builder.Configuration.GetValue<string>("TableStorageConnectionString")
                                   ?? throw new Exception("No 'TableStorageConnectionString' was provided. Use User Secrets or specify via environment variable.");

    runtimeConfiguration.EventSourcing(source =>
    {
        source.Stream(nameof(OrderBooking.OrderBooking),
            from => from.AzureTableStorage(connectionString, nameof(OrderBooking.OrderBooking)),
            into =>
            {
                into.Aggregate<OrderBooking.OrderBooking>()
                    .EnableTransientChannel<NotifySeller>();

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

app.UseOrderBooking((builder) => builder.MapGroup("api/orderbooking").WithTags("Bookings"));

app.Run();
