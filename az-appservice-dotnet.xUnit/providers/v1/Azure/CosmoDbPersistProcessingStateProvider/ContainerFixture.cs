using az_appservice_dotnet.providers.Azure.v1;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.CosmoDbPersistProcessingStateProvider;

public class ContainerFixture : IDisposable
{
    private readonly Database _database;

    public ContainerFixture()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .Build();

        string? endpointUri = config.GetSection("CosmosDb")["EndPointUri"];
        if (endpointUri == null)
            throw new Exception("App.config is missing the EndPointUri setting");

        string? primaryKey = config.GetSection("CosmosDb")["PrimaryKey"];
        if (primaryKey == null)
            throw new Exception("App.config is missing the PrimaryKey setting");

        var client = new CosmosClient(endpointUri, primaryKey, new CosmosClientOptions());
        var databaseTask = client.CreateDatabaseIfNotExistsAsync("AkvTraining_Test");
        databaseTask.Wait();
        _database = databaseTask.Result.Database;
    }

    public void Dispose()
    {
        // Clear();
    }

    public CosmosDbPersistProcessingStateProvider GetProvider(string containerSuffix)
    {
        var containerTask = _database.CreateContainerIfNotExistsAsync("_test_states_" + containerSuffix, "/taskId");
        containerTask.Wait();
        var container = containerTask.Result.Container;
        // clean everything before starting
        Clear(container);
        return new CosmosDbPersistProcessingStateProvider(container);
    }

    public void Clear(CosmosDbPersistProcessingStateProvider provider, bool deleteContainer = false)
    {
        Clear(provider._container, deleteContainer);
    }

    private void Clear(Container container, bool deleteContainer = false)
    {
        if (deleteContainer)
        {
            var task = container.DeleteContainerAsync();
            task.Wait();
        }
        else
        {
            var iterator = container
                .GetItemQueryIterator<
                    CosmosDbPersistProcessingStateProvider.CosmosState>();
            while (iterator.HasMoreResults)
            {
                var responseTask = iterator.ReadNextAsync();
                responseTask.Wait();
                var response = responseTask.Result;
                foreach (var cs in response)
                {
                    container
                        .DeleteItemAsync<
                            CosmosDbPersistProcessingStateProvider.CosmosState>(
                            cs.Id, new PartitionKey(cs.TaskId))
                        .Wait();
                }
            }
        }
    }
}

[CollectionDefinition("CosmosContainer collection")]
public class ContainerCollection : ICollectionFixture<ContainerFixture>
{
}