using MessageHandler.EventSourcing;
using MessageHandler.EventSourcing.AzureTableStorage;
using MessageHandler.Runtime;
using OrderBooking.Projections;

public static class MessageHandlerConfiguration
{
    public static HandlerRuntimeConfiguration AddHandlerRuntime(this IServiceCollection services)
    {
        var runtimeConfiguration = new HandlerRuntimeConfiguration(services);
        runtimeConfiguration.HandlerName("orderbooking");

        return runtimeConfiguration;
    }

    public static HandlerRuntimeConfiguration AddEventSource(this HandlerRuntimeConfiguration runtimeConfiguration, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("TableStorageConnectionString")
                                    ?? throw new Exception("No 'TableStorageConnectionString' was provided. Use User Secrets or specify via environment variable.");

        var eventSource = new AzureTableStorageEventSource(connectionString, table: nameof(OrderBooking));

        var eventsourcingConfiguration = new EventsourcingConfiguration(runtimeConfiguration);        
        eventsourcingConfiguration.UseEventSource(eventSource);

        eventsourcingConfiguration.EnableProjections(typeof(BookingProjection));

        eventsourcingConfiguration.RegisterEventsourcingRuntime();

        return runtimeConfiguration;
    }

}