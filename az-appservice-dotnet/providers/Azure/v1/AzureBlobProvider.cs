using az_appservice_dotnet.services.v1.Blob.dependencies;
using Azure.Storage.Blobs;

namespace az_appservice_dotnet.providers.Azure.v1;

public class AzureBlobProvider : IBlobProvider
{
    private readonly BlobContainerClient _containerClient;

    public AzureBlobProvider(IConfiguration configuration)
    {
        string? connectionString = configuration.GetSection("BlobStorage")["ConnectionString"];
        if (connectionString == null)
            throw new Exception("Configuration is missing the PrimaryKey setting (BlobStorage:ConnectionString)");
        var containerName = configuration.GetSection("BlobStorage")["Container"] ?? "processed-files";
        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    public AzureBlobProvider(BlobContainerClient containerClient)
    {
        _containerClient = containerClient;
    }

    public Task<Uri> UploadBlobAsync(string name, string localFilePath)
    {
        var blobClient = _containerClient.GetBlobClient(name);
        return blobClient.UploadAsync(localFilePath, true)
            .ContinueWith(task =>
            {
                if (task.Exception != null) throw task.Exception;
                return blobClient.Uri;
            });
    }

    public Task<bool> DownloadBlobAsync(string name, string localFilePath)
    {
        var blobClient = _containerClient.GetBlobClient(name);
        return blobClient.DownloadToAsync(localFilePath)
            .ContinueWith(task =>
            {
                if (task.Exception != null) throw task.Exception;
                return true;
            });
    }
}