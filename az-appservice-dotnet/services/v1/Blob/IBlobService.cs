namespace az_appservice_dotnet.services.v1.Blob;

public interface IBlobService
{
    Task<Uri> UploadBlobAsync(string name, string localFilePath);
    Task<bool> DownloadBlobAsync(string name, string localFilePath);
}