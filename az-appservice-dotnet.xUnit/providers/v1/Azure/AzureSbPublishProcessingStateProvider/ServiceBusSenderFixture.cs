using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.AzureSbPublishProcessingStateProvider;

public class ServiceBusSenderFixture : IDisposable
{
    private readonly ServiceBusClient _client;
    private readonly string _topicName;
    private readonly ServiceBusClient _clientProcessor;
    private readonly List<ServiceBusProcessor> _processors = new();

    public ServiceBusSenderFixture()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .Build();

        string? connectionString = configuration.GetSection("ServiceBus")["ConnectionString"];
        if (connectionString == null)
            throw new("Configuration is missing the EndPointUri setting (ServiceBus:ConnectionString)");

        _topicName = (configuration.GetSection("ServiceBus")["TopicName"] ?? "process-files") + "-test";
        _client = new(connectionString);
        _clientProcessor = new(connectionString);
    }

    public void Dispose()
    {
        foreach (var processor in _processors)
        {
            processor.StopProcessingAsync().Wait();
            processor.DisposeAsync().AsTask().Wait();
        }

        _client.DisposeAsync().AsTask().Wait();
    }

    public ServiceBusSender GetSender()
    {
        return _client.CreateSender(_topicName);
    }

    private ServiceBusProcessor GetProcessor(
        Func<ProcessMessageEventArgs, ProcessMessageEventArgs> onMessage,
        Func<ProcessErrorEventArgs, ProcessErrorEventArgs> onError)
    {
        var processor = _clientProcessor.CreateProcessor(_topicName, "subscription1");
        processor.ProcessMessageAsync += (args) =>
        {
            onMessage.Invoke(args);
            return args.CompleteMessageAsync(args.Message);
        };
        processor.ProcessErrorAsync += (args) =>
        {
            onError.Invoke(args);
            return Task.CompletedTask;
        };
        _processors.Add(processor);
        return processor;
    }

    public Tuple<List<ProcessMessageEventArgs>,List<ProcessErrorEventArgs>,ServiceBusProcessor> RunProcessor()
    {
        List<ProcessMessageEventArgs> messages = new();
        List<ProcessErrorEventArgs> errors = new();
        var processor = GetProcessor((args) =>
        {
            messages.Add(args);
            return args;
        }, (args) =>
        {
            errors.Add(args);
            return args;
        });
        processor.StartProcessingAsync();
        // skip outstanding messages
        Thread.Sleep(1000);
        messages.Clear();
        errors.Clear();
        return new(messages, errors, processor);
    }

    public az_appservice_dotnet.providers.Azure.v1.AzureSbPublishProcessingStateProvider GetProvider()
    {
        return new(GetSender());
    }
}

[CollectionDefinition("ServiceBusSender collection")]
public class ContainerCollection : ICollectionFixture<ServiceBusSenderFixture>
{
}