using System.Runtime.CompilerServices;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo("az-appservice-dotnet.xUnit")]
namespace az_appservice_dotnet.services.v1;

public class CosmoDbProcessingStateService : IProcessingStateService
{
    /*
    public static async ValueTask<CosmoDbProcessingStateService> GetInstance(
        string databaseId = "AkvTraining",
        string containerId = "States")
    {
        string? endpointUri = ConfigurationManager.AppSettings["EndPointUri"];
        if (endpointUri == null)
            throw new Exception("App.config is missing the EndPointUri setting");

        string? primaryKey = ConfigurationManager.AppSettings["PrimaryKey"];
        if (primaryKey == null)
            throw new Exception("App.config is missing the PrimaryKey setting");

        var client = new CosmosClient(endpointUri, primaryKey, new CosmosClientOptions());
        var database = (await client.CreateDatabaseIfNotExistsAsync(databaseId)).Database;
        var container = (await database.CreateContainerIfNotExistsAsync(containerId, "/partitionKey")).Container;
        var service = new CosmoDbProcessingStateService(container);
        return service;
    }*/

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

        public CosmosState(IProcessingStateService.State state)
        {
            Id = state.Id;
            TaskId = state.TaskId;
            Status = state.Status;
            OriginalFileUrl = state.OriginalFileUrl;
            ProcessedFileUrl = state.ProcessedFileUrl;
            FileName = state.FileName;
        }

        public CosmosState(string id, TaskId taskId, IProcessingStateService.Status status, string? originalFileUrl,
            string? processedFileUrl, string fileName)
        {
            Id = id;
            TaskId = taskId;
            Status = status;
            OriginalFileUrl = originalFileUrl;
            ProcessedFileUrl = processedFileUrl;
            FileName = fileName;
        }

        public static implicit operator IProcessingStateService.State(CosmosState state) =>
            new(state.Id, state.TaskId, state.Status, state.OriginalFileUrl,
                state.ProcessedFileUrl, state.FileName);
    }

    private readonly Container _container;

    private const string DatabaseId = "AkvTraining";
    private const string ContainerId = "States";

    public CosmoDbProcessingStateService(IConfiguration configuration)
    {
        string? endpointUri = configuration.GetSection("CosmosDb")["EndPointUri"];
        if (endpointUri == null)
            throw new Exception("App.config is missing the EndPointUri setting");

        string? primaryKey = configuration.GetSection("CosmosDb")["PrimaryKey"];
        if (primaryKey == null)
            throw new Exception("App.config is missing the PrimaryKey setting");

        var client = new CosmosClient(endpointUri, primaryKey, new CosmosClientOptions());
        var databaseTask = client.CreateDatabaseIfNotExistsAsync(DatabaseId);
        databaseTask.Wait();
        var database = databaseTask.Result.Database;
        var containerTask = database.CreateContainerIfNotExistsAsync(ContainerId, "/taskId");
        containerTask.Wait();
        _container = containerTask.Result.Container;
    }

    public CosmoDbProcessingStateService(Container container)
    {
        _container = container;
    }

    public async ValueTask<IProcessingStateService.State> CreateInitialState(TaskId taskId, string fileName)
    {
        var state = new CosmosState(
            Guid.NewGuid().ToString(),
            taskId,
            IProcessingStateService.Status.Created,
            null,
            null,
            fileName);
        await _container.CreateItemAsync(state, new PartitionKey(state.TaskId));
        return state;
    }

    public ValueTask<IProcessingStateService.StateId> MoveToUploadingStateAsync(TaskId id)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IProcessingStateService.StateId> MoveToWaitingForProcessingStateAsync(TaskId id)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IProcessingStateService.StateId> MoveToProcessingStateAsync(TaskId id)
    {
        throw new NotImplementedException();
    }

    public ValueTask<IProcessingStateService.StateId> MoveToCompletedStateAsync(TaskId id, string? processedFileUrl)
    {
        throw new NotImplementedException();
    }

    public ValueTask<Dictionary<IProcessingStateService.StateId, IProcessingStateService.Status>>
        GetStatesDictionaryAsync()
    {
        throw new NotImplementedException();
    }
}