namespace az_appservice_dotnet.services.v1.Blob;

public interface IBlobService
{
    Task<Uri> StoreBlobAsync(string name, string localFilePath);
}