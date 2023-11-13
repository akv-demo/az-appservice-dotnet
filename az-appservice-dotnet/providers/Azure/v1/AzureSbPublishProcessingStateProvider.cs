using az_appservice_dotnet.services;
using az_appservice_dotnet.services.v1.State;
using az_appservice_dotnet.services.v1.State.dependencies;
using Azure.Messaging.ServiceBus;

namespace az_appservice_dotnet.providers.Azure.v1;

public class AzureSbPublishProcessingStateProvider: IPublishProcessingStateProvider
{
    private ServiceBusSender _sender;
    public AzureSbPublishProcessingStateProvider(IConfiguration configuration)
    {
        string? connectionString = configuration.GetSection("ServiceBus")["ConnectionString"];
        if (connectionString == null)
            throw new Exception("Configuration is missing the EndPointUri setting (ServiceBus:ConnectionString)");
        
        string topicName = configuration.GetSection("ServiceBus")["TopicName"] ?? "process-files";
        
        var client = new ServiceBusClient(connectionString);
        _sender = client.CreateSender(topicName);
    }
    
    public AzureSbPublishProcessingStateProvider(ServiceBusSender sender)
    {
        _sender = sender;
    }
    public Task<IProcessingStateService.State> PublishStateAsync(in IProcessingStateService.State state)
    {
        var state1 = state;
        return _sender.SendMessageAsync(new ServiceBusMessage(state.Id))
            .ContinueWith(_ => state1);
    }
}