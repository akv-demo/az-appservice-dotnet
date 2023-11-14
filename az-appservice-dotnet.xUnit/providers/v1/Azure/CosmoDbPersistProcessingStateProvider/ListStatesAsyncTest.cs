using az_appservice_dotnet.services.v1.State;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.CosmoDbPersistProcessingStateProvider;

[Collection("CosmosContainer collection")]
public class ListStatesAsyncTest: Base
{
    public ListStatesAsyncTest(ContainerFixture containerFixture) : base(nameof(ListStatesAsyncTest), containerFixture)
    {
    }

    [Fact]
    public async Task Should_List2Items()
    {
        // Arrange
        var created1 = await _provider.CreateStateAsync(
                new IProcessingStateService.State(
                    1,
                    IProcessingStateService.Status.Created,
                    null,
                    null,
                    "file1.txt",
                    null)
            );
        var created2 = await _provider.CreateStateAsync(
                new IProcessingStateService.State(
                    2,
                    IProcessingStateService.Status.Created,
                    null,
                    null,
                    "file1.txt",
                    null)
            );
        // Act

        var read = await _provider.ListStatesAsync();


        // Assert
        Assert.Equal(2, read.Count());
        Assert.True(read.ContainsKey(created1.Id));
        Assert.True(read.ContainsKey(created2.Id));
    }
    
    // test list is empty
    [Fact]
    public async Task Should_List0Items()
    {
        // Arrange
        // Act
        var read = await _provider.ListStatesAsync();
        // Assert
        Assert.Empty(read);
    }

}