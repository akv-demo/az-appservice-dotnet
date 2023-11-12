namespace az_appservice_dotnet.services.v1;

public class AzureBlobService: IBlobService
{
    private readonly string _defaultContainer;
    
    public AzureBlobService(string defaultContainer)
    {
        _defaultContainer = defaultContainer;
    }
    
    public Task<bool> StoreBlobAsync(string name, string localFilePath)
    {
        throw new NotImplementedException();
    }

    public Task<bool> StoreBlobAsync(string container, string name, string localFilePath)
    {
        throw new NotImplementedException();
    }
}