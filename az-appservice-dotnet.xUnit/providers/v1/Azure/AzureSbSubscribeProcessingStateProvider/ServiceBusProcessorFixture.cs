using az_appservice_dotnet.providers.Azure.v1;
using az_appservice_dotnet.services.v1.State.dependencies;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.AzureSbSubscribeProcessingStateProvider;

public class ServiceBusProcessorFixture : IDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusClient _clientSender;
    private readonly string _topicName;
    private readonly string _subscriptionName;
    private readonly IPersistProcessingStateProvider _persistProcessingStateProvider;

    public ServiceBusProcessorFixture()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .Build();

        string? connectionString = configuration.GetSection("ServiceBus")["ConnectionString"];
        if (connectionString == null)
            throw new("Configuration is missing the EndPointUri setting (ServiceBus:ConnectionString)");

        _topicName = (configuration.GetSection("ServiceBus")["TopicName"] ?? "process-files") + "-test-subscribe";
        _subscriptionName =
            (configuration.GetSection("ServiceBus")["SubscriptionName"] ?? "process-files-subscription");
        _client = new(connectionString);
        _clientSender = new(connectionString);
        _persistProcessingStateProvider = new CosmosDbPersistProcessingStateProvider(configuration);
    }

    public void Dispose()
    {
        _client.DisposeAsync().AsTask().Wait();
        _clientSender.DisposeAsync().AsTask().Wait();
    }

    public ServiceBusSender GetSender()
    {
        return _clientSender.CreateSender(_topicName);
    }
    private ServiceBusProcessor GetProcessor()
    {
        return _client.CreateProcessor(_topicName, _subscriptionName);
    }

    public az_appservice_dotnet.providers.Azure.v1.AzureSbSubscribeProcessingStateProvider GetProvider()
    {
        return new(GetProcessor(), _persistProcessingStateProvider);
    }

    public IPersistProcessingStateProvider PersistProcessingStateProvider()
    {
        return _persistProcessingStateProvider;
    }
}

[CollectionDefinition("ServiceBusProcessor collection")]
public class ContainerCollection : ICollectionFixture<ServiceBusProcessorFixture>
{
}