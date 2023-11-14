namespace az_appservice_dotnet.services.v1.Blob.dependencies;

public interface IBlobProvider
{
    Task<Uri> UploadBlobAsync(string name, string localFilePath);
    Task<Boolean> DownloadBlobAsync(string name, string localFilePath);
}