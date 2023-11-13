using az_appservice_dotnet.services.v1.Blob.dependencies;

namespace az_appservice_dotnet.services.v1.Blob;

/**
 * <summary>
 * This is a wrapper around a blob provider to implement some possible business logic common
 * for differnt blob providers.
 * </summary>
 */
public class BlobService: IBlobService
{
private readonly IBlobProvider _blobProvider;

    public BlobService(IBlobProvider blobProvider)
    {
        _blobProvider = blobProvider;
    }

    public Task<Uri> StoreBlobAsync(string name, string localFilePath)
    {
        return _blobProvider.StoreBlobAsync(name, localFilePath);
    }
}