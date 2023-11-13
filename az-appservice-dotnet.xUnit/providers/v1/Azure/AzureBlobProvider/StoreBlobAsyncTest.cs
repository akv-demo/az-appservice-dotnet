using az_appservice_dotnet.services;
using az_appservice_dotnet.services.v1.UploadedFiles;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.AzureBlobProvider;

[Collection("AzureBlobService collection")]

public class StoreBlobAsyncTest: IClassFixture<FileProviderFixture>
{
    readonly ContainerFixture _containerFixture;
    readonly FileProviderFixture _fileProviderFixture;
    
    public StoreBlobAsyncTest(ContainerFixture containerFixture, FileProviderFixture fileProviderFixture)
    {
        _containerFixture = containerFixture;
        _fileProviderFixture = fileProviderFixture;
    }
    
    [Fact]
    public async Task Should_ReturnUri()
    {
        // Arrange
        var provider = _containerFixture.GetProvider();
        var fileObject = _fileProviderFixture.GetFileObject("test.bin",1);
        // Act
        var actual = await provider.StoreBlobAsync(fileObject.Name, fileObject.Path);
        _containerFixture.DisposableBag.Add(fileObject.Name);
        // Assert
        var readResponse = await _containerFixture.ContainerClient.GetBlobClient(fileObject.Name).ExistsAsync();
        Assert.True(readResponse.Value);
        
        Assert.IsType<Uri>(actual);
        var expectedUri = _containerFixture.ContainerClient.Uri.ToString() + '/' + fileObject.Name;
        Assert.Equal(expectedUri, actual.ToString());
    }
    
    // Test for exception
    [Fact]
    public async Task Should_ThrowException()
    {
        // Arrange
        var provider = _containerFixture.GetProvider();
        var fileObject = new IFileProviderService.FileObject("test.bin","/nonexistent");
        // Act
        var exception = await Record.ExceptionAsync(() => provider.StoreBlobAsync(fileObject.Name, fileObject.Path));
        // Assert
        Assert.NotNull(exception);
        Assert.IsType<AggregateException>(exception);
    }
}