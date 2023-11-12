using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace az_appservice_dotnet.xUnit.services.v1.CosmoDbProcessingStateService;

public class ContainerFixture : IDisposable
{
    public readonly Container Container;

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
        var databaseTask = client.CreateDatabaseIfNotExistsAsync("AkvTraining");
        databaseTask.Wait();
        var database = databaseTask.Result.Database;
        var containerTask = database.CreateContainerIfNotExistsAsync("_test_States", "/taskId");
        containerTask.Wait();
        Container = containerTask.Result.Container;
    }

    public void Dispose()
    {
        Container.DeleteContainerAsync().Wait();
    }

    public az_appservice_dotnet.services.v1.CosmoDbProcessingStateService GetService()
    {
        return new az_appservice_dotnet.services.v1.CosmoDbProcessingStateService(Container);
    }
}