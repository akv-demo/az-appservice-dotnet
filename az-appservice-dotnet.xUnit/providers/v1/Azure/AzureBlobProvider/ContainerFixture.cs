using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.AzureBlobProvider;

public class ContainerFixture : IDisposable
{
    public readonly BlobContainerClient ContainerClient;
    public readonly List<string> DisposableBag = new();

    public ContainerFixture()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .Build();

        string? connectionString = config.GetSection("BlobStorage")["ConnectionString"];
        if (connectionString == null)
            throw new Exception("Configuration is missing the PrimaryKey setting (BlobStorage:ConnectionString)");
        var containerName = "processed-files-test";
        var blobServiceClient = new BlobServiceClient(connectionString);
        ContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    public void Dispose()
    {
        foreach (var blob in DisposableBag)
        {
            ContainerClient.DeleteBlob(blob);
        }
    }

    public az_appservice_dotnet.providers.Azure.v1.AzureBlobProvider GetProvider()
    {
        return new az_appservice_dotnet.providers.Azure.v1.AzureBlobProvider(ContainerClient);
    }
}

[CollectionDefinition("AzureBlobService collection")]
public class ContainerCollection : ICollectionFixture<ContainerFixture>
{
}