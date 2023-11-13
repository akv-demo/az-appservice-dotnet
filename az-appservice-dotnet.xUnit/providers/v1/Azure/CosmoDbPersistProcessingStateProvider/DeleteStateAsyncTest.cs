using az_appservice_dotnet.services.v1.State;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.CosmoDbPersistProcessingStateProvider;

[Collection("CosmosContainer collection")]
public class DeleteStateAsyncTest : Base
{
    public DeleteStateAsyncTest(ContainerFixture containerFixture) : base(nameof(DeleteStateAsyncTest), containerFixture)
    {
    }
    
    [Fact]
    public async Task Should_DeleteItem()
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
        // Act
        await _provider.DeleteStateAsync(created);
        // Assert
        var list = await _provider.ListStatesAsync();
        Assert.Empty(list);
    }
    
}