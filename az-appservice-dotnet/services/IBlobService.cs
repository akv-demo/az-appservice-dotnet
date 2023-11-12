namespace az_appservice_dotnet.services;

public interface IBlobService
{
    Task<bool> StoreBlobAsync(string name, string localFilePath);
    Task<bool> StoreBlobAsync(string container, string name, string localFilePath);
}