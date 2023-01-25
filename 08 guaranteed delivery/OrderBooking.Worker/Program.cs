using MessageHandler.Runtime;
using MessageHandler.Runtime.AtomicProcessing;
using OrderBooking.Events;
using OrderBooking.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging();

        services.AddMessageHandler("orderbooking.worker", runtimeConfiguration =>
        {
            var serviceBusConnectionString = hostContext.Configuration.GetValue<string>("ServiceBusConnectionString")
                                           ?? throw new Exception("No 'ServiceBusConnectionString' was provided. Use User Secrets or specify via environment variable.");

            runtimeConfiguration.AtomicProcessingPipeline(pipeline =>
            {
                pipeline.PullMessagesFrom(p => p.Topic(name: "orderbooking.events", subscription: "orderbooking.worker", serviceBusConnectionString));
                pipeline.DetectTypesInAssembly(typeof(BookingStarted).Assembly);
                pipeline.HandleMessagesWith<SendNotificationMail>();
            });            
        });
    })
    .Build();

await host.RunAsync();