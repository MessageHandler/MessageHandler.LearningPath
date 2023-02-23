using MessageHandler.EventSourcing;
using MessageHandler.EventSourcing.AzureTableStorage;
using MessageHandler.Runtime;
using MessageHandler.Runtime.AtomicProcessing;
using OrderBooking.Events;
using OrderBooking.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging();

        services.AddPostmark();

        var storageConnectionString = hostContext.Configuration.GetValue<string>("TableStorageConnectionString")
                                   ?? throw new Exception("No 'TableStorageConnectionString' was provided. Use User Secrets or specify via environment variable.");

        var serviceBusConnectionString = hostContext.Configuration.GetValue<string>("ServiceBusConnectionString")
                                         ?? throw new Exception("No 'ServiceBusConnectionString' was provided. Use User Secrets or specify via environment variable.");

        var searchEndpoint = hostContext.Configuration.GetValue<string>("SearchEndpoint")
                             ?? throw new Exception("No 'SearchEndpoint' connection string was provided. Use User Secrets or specify via environment variable.");
        var apiKey = hostContext.Configuration.GetValue<string>("SearchApiKey")
                     ?? throw new Exception("No 'azuresearchapikey' connection string was provided. Use User Secrets or specify via environment variable.");

        services.AddSearch(searchEndpoint, apiKey);

        var sqlServerConnectionString = hostContext.Configuration.GetValue<string>("SqlServerConnectionString")
                               ?? throw new Exception("No 'SqlServerConnectionString' was provided. Use User Secrets or specify via environment variable.");

        services.AddSingleton<IProcessAvailableConfirmationMails>(new AvailableConfirmationMails(sqlServerConnectionString));
        services.AddHostedService<SendConfirmationMail>();

        services.AddMessageHandler("orderbooking.worker", runtimeConfiguration =>
        {            
            runtimeConfiguration.AtomicProcessingPipeline(pipeline =>
            {
                pipeline.PullMessagesFrom(p => p.Topic(name: "orderbooking.events", subscription: "orderbooking.worker", serviceBusConnectionString));
                pipeline.DetectTypesInAssembly(typeof(BookingStarted).Assembly);
                pipeline.HandleMessagesWith<SendNotificationMail>();
            });

            runtimeConfiguration.EventSourcing(source =>
            {                
                source.Stream("OrderBooking",
                    from => from.AzureTableStorage(storageConnectionString, "OrderBooking"),
                    into => into.Projection<ProjectToSearch>());                
            });

            runtimeConfiguration.AtomicProcessingPipeline(pipeline =>
            {
                pipeline.PullMessagesFrom(p => p.Topic(name: "orderbooking.events", subscription: "orderbooking.indexing", serviceBusConnectionString));
                pipeline.DetectTypesInAssembly(typeof(BookingStarted).Assembly);
                pipeline.HandleMessagesWith<IndexSalesOrder>();
                pipeline.HandleMessagesWith<IndexConfirmedSalesOrder>();
            });
        });
    })
    .Build();

await host.RunAsync();