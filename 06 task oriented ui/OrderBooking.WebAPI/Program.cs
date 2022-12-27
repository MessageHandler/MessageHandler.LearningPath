using MessageHandler.EventSourcing;
using MessageHandler.EventSourcing.AzureTableStorage;
using MessageHandler.Runtime;
using OrderBooking.Projections;

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
                into.Aggregate<OrderBooking.OrderBooking>();

                into.Projection<BookingProjection>();
            });
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyOrigin()
              .AllowAnyHeader();
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

app.Run();
