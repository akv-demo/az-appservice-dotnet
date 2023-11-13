using az_appservice_dotnet.services;
using az_appservice_dotnet.services.v1;
using az_appservice_dotnet.services.v1.UploadedFiles;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.AzureBlobProvider;

public class FileProviderFixture : IDisposable
{
    private readonly IFileProviderService _fileProviderService = new FakeFileProviderService();

    private readonly List<string> _files = new();

    public IFileProviderService.FileObject GetFileObject(string name, uint size)
    {
        var fileObejct = _fileProviderService.GetFileObject(name, size);
        _files.Add(fileObejct.Path);
        return fileObejct;
    }

    public void Dispose()
    {
        foreach (var file in _files)
        {
            if (File.Exists(file))
                File.Delete(file);
        }
    }
}