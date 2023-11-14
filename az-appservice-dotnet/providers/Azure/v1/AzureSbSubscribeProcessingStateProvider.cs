using az_appservice_dotnet.services.v1.State;
using az_appservice_dotnet.services.v1.State.dependencies;
using Azure.Messaging.ServiceBus;

namespace az_appservice_dotnet.providers.Azure.v1;

public class AzureSbSubscribeProcessingStateProvider : ISubscribeProcessingStateProvider
{
    private readonly ServiceBusProcessor _processor;
    private readonly object _lockHandlers = new();
    private readonly List<IProcessingStateService.StateChangeHandler> _handlers = new();

    public AzureSbSubscribeProcessingStateProvider(IConfiguration configuration,
        IPersistProcessingStateProvider persistProcessingStateProvider)
    {
        // It duplicates the code from AzureSbPublishProcessingStateProvider
        // but is intentional to have a flexibility to use different connection strings
        string? connectionString = configuration.GetSection("ServiceBus")["ConnectionString"];
        if (connectionString == null)
            throw new Exception("Configuration is missing the EndPointUri setting (ServiceBus:ConnectionString)");

        string topicName = configuration.GetSection("ServiceBus")["TopicName"] ?? "process-files";

        var subscriptionName =
            (configuration.GetSection("ServiceBus")["SubscriptionName"] ?? "process-files-subscription");
        var client = new ServiceBusClient(connectionString);
        _processor = client.CreateProcessor(topicName, subscriptionName);
        ConfigureProcessor(persistProcessingStateProvider);
    }

    public AzureSbSubscribeProcessingStateProvider(ServiceBusProcessor processor, IPersistProcessingStateProvider persistProcessingStateProvider )
    {
        _processor = processor;
        ConfigureProcessor(persistProcessingStateProvider);
    }
    private void ConfigureProcessor(IPersistProcessingStateProvider persistProcessingStateProvider)
    {
        _processor.ProcessMessageAsync += (args) =>
        {
            var stateS = args.Message.Body.ToString().Split(':');
            if (stateS.Length != 2)
                return args.CompleteMessageAsync(args.Message);
            var stateId = stateS[0];
            if (!int.TryParse(stateS[1], out var taskId))
            {
                return args.CompleteMessageAsync(args.Message);
            }

            persistProcessingStateProvider.ReadStateAsync(stateId, taskId)
                .ContinueWith((task) =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        var state = task.Result;

                        IProcessingStateService.StateChangeHandler[] handlers;
                        lock (_lockHandlers)
                        {
                            handlers = _handlers.ToArray();
                        }

                        foreach (var handler in handlers)
                        {
                            handler.Invoke(state);
                        }
                    }
                });
            return args.CompleteMessageAsync(args.Message);
        };
        _processor.ProcessErrorAsync += (_) =>
        {
            // TODO: log error
            return Task.CompletedTask;
        };
        
    }

    public void AddStateChangeHandler(IProcessingStateService.StateChangeHandler handler)
    {
        lock (_lockHandlers)
        {
            _handlers.Add(handler);
            if (_handlers.Count == 1)
            {
                _processor.StartProcessingAsync();
            }
        }
    }

    public void RemoveStateChangeHandler(IProcessingStateService.StateChangeHandler handler)
    {
        lock (_lockHandlers)
        {
            _handlers.Remove(handler);
            if (_handlers.Count == 0)
            {
                _processor.StopProcessingAsync();
            }
        }
    }
}