namespace az_appservice_dotnet.nUnit.services.v1.FakeFileProviderService;

public class GetFileObjectTest
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
    public void GetFileObject_shouldReturnFilePath()
    {
        // Arrange
        var sut = new az_appservice_dotnet.services.v1.FakeImageProviderService();
        // Act
        var file = sut.GetFileObject("test", 2);
        _filePaths.Add(file.Path);
        // Assert
        Assert.True(File.Exists(file.Path));
        var fileInfo = new FileInfo(file.Path);
        Assert.True(2 * 1024 * 1024 == fileInfo.Length);
    }
}