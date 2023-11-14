using az_appservice_dotnet.services.v1.State;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.CosmoDbPersistProcessingStateProvider;

[Collection("CosmosContainer collection")]
public class ReadStateAsyncTest : Base
{
    public ReadStateAsyncTest(ContainerFixture containerFixture) : base(nameof(ReadStateAsyncTest), containerFixture)
    {
    }

    [Fact]
    public async Task Should_ReadState()
    {
        // Arrange
        var created = await _provider.CreateStateAsync(
            new IProcessingStateService.State(
                1,
                IProcessingStateService.Status.Created,
                null,
                null,
                "file1.txt",
                null)
        );
        // Act
        var read = await _provider.ReadStateAsync(created.Id, created.TaskId);
        // Assert
        Assert.Equal(created.Id, read.Id);
        Assert.Equal(created.TaskId, read.TaskId);
        Assert.Equal(created.Status, read.Status);
        Assert.Equal(created.OriginalFileUrl, read.OriginalFileUrl);
        Assert.Equal(created.ProcessedFileUrl, read.ProcessedFileUrl);
        Assert.Equal(created.FileName, read.FileName);
        Assert.Equal(created.FailureReason, read.FailureReason);
    }
    
    [Fact]
    public async Task Should_ThrowNotFound()
    {
        // Arrange
        // Act
        var exception = await Record.ExceptionAsync(() => _provider.ReadStateAsync("non-existing", 0));
        // Assert
        Assert.IsType<AggregateException>(exception);
    }
}