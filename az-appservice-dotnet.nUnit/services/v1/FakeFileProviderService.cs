namespace az_appservice_dotnet.nUnit.services.v1;

public class FakeFileProviderService
{
    // a list of strings to hold the file paths
    private List<String> _filePaths = new();

    [SetUp]
    public void Setup()
    {
    }

    // clean
    [TearDown]
    public void TearDown()
    {
        // delete the files which were created by the tests
        foreach (var filePath in _filePaths)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [Test]
    public void FakeFileProviderService_shouldReturnFilePath()
    {
        // Arrange
        var sut = new az_appservice_dotnet.services.v1.FakeImageProviderService(2);
        // Act
        var filePath = sut.GetFilePath();
        _filePaths.Add(filePath);
        // Assert
        Assert.True(File.Exists(filePath));
        var fileInfo = new FileInfo(filePath);
        Assert.True(2 * 1024 * 1024 == fileInfo.Length);
    }
}