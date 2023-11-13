using System.Net;
using az_appservice_dotnet.providers.Azure.v1;
using az_appservice_dotnet.services.v1.State;
using Microsoft.Azure.Cosmos;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.CosmoDbPersistProcessingStateProvider;

[Collection("CosmosContainer collection")]
public class CreateInitialStateTest: Base
{
    public CreateInitialStateTest(ContainerFixture containerFixture) : base(nameof(CreateInitialStateTest), containerFixture)
    {
    }

    [Fact]
    public async Task Should_CreateAndAssignId()
    {
        // Arrange
        var state = new IProcessingStateService.State(
            777,
            IProcessingStateService.Status.Created,
            null,
            null,
            "file1.txt",
            null);
        // Act
        var actual = await _provider.CreateStateAsync(state);
        var idException = Record.Exception(() => actual.Id);
        // Assert
        Assert.Null(idException);
        Assert.IsType<IProcessingStateService.State>(actual);
        Assert.Equal((int)state.TaskId, (int)actual.TaskId);
        Assert.Equal(state.FileName, actual.FileName);
        Assert.Equal(state.Status, actual.Status);
        Assert.Equal(state.OriginalFileUrl, actual.OriginalFileUrl);
        Assert.Equal(state.ProcessedFileUrl, actual.ProcessedFileUrl);
        Assert.Equal(state.FailureReason, actual.FailureReason);

        var readResponse = await _provider._container.ReadItemAsync<az_appservice_dotnet.providers.Azure.v1.CosmosDbPersistProcessingStateProvider.CosmosState>(actual.Id,
            new PartitionKey(actual.TaskId));
        Assert.Equal(HttpStatusCode.OK, readResponse.StatusCode);
        var read = readResponse.Resource;
        Assert.IsType<az_appservice_dotnet.providers.Azure.v1.CosmosDbPersistProcessingStateProvider.CosmosState>(read);
        Assert.Equal(actual.Id, read.Id);
        Assert.Equal((int)actual.TaskId, read.TaskId);
        Assert.Equal(actual.Status, read.Status);
        Assert.Equal(actual.OriginalFileUrl, read.OriginalFileUrl);
        Assert.Equal(actual.ProcessedFileUrl, read.ProcessedFileUrl);
        Assert.Equal(actual.FileName, read.FileName);
        Assert.Equal(actual.FailureReason, read.FailureReason);
    }
    
}