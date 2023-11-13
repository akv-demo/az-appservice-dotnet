using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using az_appservice_dotnet.services;
using az_appservice_dotnet.services.v1.State;
using az_appservice_dotnet.services.v1.State.dependencies;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("az-appservice-dotnet.xUnit")]

namespace az_appservice_dotnet.providers.Azure.v1;

public class CosmosDbPersistProcessingStateProvider : IPersistProcessingStateProvider
{
    protected internal struct CosmosState
    {
        [JsonProperty(PropertyName = "id")] public string Id { get; set; }

        [JsonProperty(PropertyName = "taskId")]
        public readonly int TaskId;

        [JsonProperty(PropertyName = "status")]
        public readonly IProcessingStateService.Status Status;

        [JsonProperty(PropertyName = "originalFileUrl")]
        public readonly string? OriginalFileUrl;

        [JsonProperty(PropertyName = "processedFileUrl")]
        public readonly string? ProcessedFileUrl;

        [JsonProperty(PropertyName = "fileName")]
        public readonly string FileName;

        [JsonProperty(PropertyName = "failureReason")]
        public readonly string? FailureReason;

        public CosmosState(IProcessingStateService.State state)
        {
            Id = state.Id;
            TaskId = state.TaskId;
            Status = state.Status;
            OriginalFileUrl = state.OriginalFileUrl;
            ProcessedFileUrl = state.ProcessedFileUrl;
            FileName = state.FileName;
            FailureReason = state.FailureReason;
        }

        public CosmosState(IProcessingStateService.StateId id, IProcessingStateService.State state)
        {
            Id = id;
            TaskId = state.TaskId;
            Status = state.Status;
            OriginalFileUrl = state.OriginalFileUrl;
            ProcessedFileUrl = state.ProcessedFileUrl;
            FileName = state.FileName;
            FailureReason = state.FailureReason;
        }

        public static implicit operator IProcessingStateService.State(CosmosState cs) =>
            new(cs.Id, cs.TaskId, cs.Status, cs.OriginalFileUrl,
                cs.ProcessedFileUrl, cs.FileName, cs.FailureReason);

        public static implicit operator CosmosState(IProcessingStateService.State state) =>
            new(state);
    }

    internal readonly Container _container;

    private const string DatabaseId = "AkvTraining";
    private const string ContainerId = "States";

    public CosmosDbPersistProcessingStateProvider(IConfiguration configuration)
    {
        string? endpointUri = configuration.GetSection("CosmosDb")["EndPointUri"];
        if (endpointUri == null)
            throw new Exception("Configuration is missing the EndPointUri setting (CosmosDb:EndPointUri)");

        string? primaryKey = configuration.GetSection("CosmosDb")["PrimaryKey"];
        if (primaryKey == null)
            throw new Exception("Configuration is missing the PrimaryKey setting (CosmosDb:PrimaryKey)");

        var client = new CosmosClient(endpointUri, primaryKey, new CosmosClientOptions());
        var databaseTask = client.CreateDatabaseIfNotExistsAsync(DatabaseId);
        databaseTask.Wait();
        var database = databaseTask.Result.Database;
        var containerTask = database.CreateContainerIfNotExistsAsync(ContainerId, "/taskId");
        containerTask.Wait();
        _container = containerTask.Result.Container;
    }

    // used in tests
    internal CosmosDbPersistProcessingStateProvider(Container container)
    {
        _container = container;
    }


    public Task<IProcessingStateService.State> CreateStateAsync(in IProcessingStateService.State state)
    {
        return
            _container.CreateItemAsync(
                    new CosmosState(Guid.NewGuid().ToString(), state), new PartitionKey(state.TaskId))
                .ContinueWith(cs => (IProcessingStateService.State)cs.Result.Resource);
    }

    /*
    public IProcessingStateService.State ReadStateAsync(IProcessingStateService.StateId id)
    {
        throw new Exception("CosmosDB does not implement ReadStateAsync, use ListStatesAsync instead");
    }
    */

    public Task<IProcessingStateService.State> UpdateStateAsync(in IProcessingStateService.State state)
    {
        return _container.ReplaceItemAsync<CosmosState>(state, state.Id, new PartitionKey(state.TaskId))
            .ContinueWith(t => (IProcessingStateService.State)t.Result.Resource);
    }

    public Task<IProcessingStateService.State> DeleteStateAsync(in IProcessingStateService.State state)
    {
        return _container.DeleteItemAsync<CosmosState>(state.Id, new PartitionKey(state.TaskId))
            .ContinueWith(t => (IProcessingStateService.State)t.Result.Resource);
    }

    public async Task<ImmutableArray<IProcessingStateService.State>> ListStatesAsync()
    {
        // TODO: not efficient, but works for now
        var iterator = _container.GetItemQueryIterator<CosmosState>();
        var states = new ConcurrentQueue<IProcessingStateService.State>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            foreach (var cs in response)
            {
                states.Enqueue(cs);
            }
        }

        return states.ToImmutableArray();
    }
}