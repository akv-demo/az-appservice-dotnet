namespace az_appservice_dotnet.services.v1.Blob.dependencies;

public interface IBlobProvider
{
    Task<Uri> StoreBlobAsync(string name, string localFilePath);
}