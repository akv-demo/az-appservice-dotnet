using az_appservice_dotnet.services.v1.State;
using az_appservice_dotnet.services.v1.State.dependencies;
using Azure.Messaging.ServiceBus;

namespace az_appservice_dotnet.providers.Azure.v1;

/**
 * <summary>
 * Known bug/feature: the provider makes a lookup to IPersistProcessingStateProvider in order
 * to find the state by the ID extracted from the message. By the moment the message is
 * read from the queue, the state might be already changed in the database.
 *
 * So the message sent for change to State1 can cause the propagation of the State2. It would not
 * be a problem if the State2 had not its own message sent to the queue. In this case the subscriber
 * will receive 2 notification for State2 and none for State1.
 *
 * Generally speaking, it it correct behaviour, but still might cause the problem with handling the
 * some states twice and missing the others. 
 * </summary>
 */

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