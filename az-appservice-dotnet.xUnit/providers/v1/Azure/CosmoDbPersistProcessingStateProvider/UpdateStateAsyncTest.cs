using az_appservice_dotnet.providers.Azure.v1;
using az_appservice_dotnet.services.v1.State;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.CosmoDbPersistProcessingStateProvider;

[Collection("CosmosContainer collection")]
public class UpdateStateAsyncTest: Base
{
    public UpdateStateAsyncTest(ContainerFixture containerFixture) : base(nameof(UpdateStateAsyncTest), containerFixture)
    {
    }
    
    [Fact]
    public async Task Should_UpdateState()
    {
        // Arrange
        var created = await _provider.CreateStateAsync(
            new IProcessingStateService.State(
                "a",
                1,
                IProcessingStateService.Status.Created,
                null,
                null,
                "file1.txt",
                null)
        );
        var updated = new IProcessingStateService.State(
            created.Id,
            created.TaskId, // must be the same because of partition key
            IProcessingStateService.Status.Processing,
            null,
            null,
            "file2.txt",
            null);
        // Act
        var actual = await _provider.UpdateStateAsync(updated);
        // Assert
        Assert.Equal(updated.Id, actual.Id);
        Assert.Equal(updated.TaskId, actual.TaskId);
        Assert.Equal(updated.Status, actual.Status);
        Assert.Equal(updated.OriginalFileUrl, actual.OriginalFileUrl);
        Assert.Equal(updated.ProcessedFileUrl, actual.ProcessedFileUrl);
        Assert.Equal(updated.FileName, actual.FileName);
        Assert.Equal(updated.FailureReason, actual.FailureReason);

        var list = await _provider.ListStatesAsync();
        Assert.Single(list);
    }
}