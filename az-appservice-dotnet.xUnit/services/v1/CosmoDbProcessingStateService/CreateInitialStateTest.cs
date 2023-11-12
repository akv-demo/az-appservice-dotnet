using System.Net;
using az_appservice_dotnet.services;
using Microsoft.Azure.Cosmos;

namespace az_appservice_dotnet.xUnit.services.v1.CosmoDbProcessingStateService;

[Collection("CosmosContainer collection")]
public class CreateInitialStateTest
{
    readonly ContainerFixture _containerFixture;

    public CreateInitialStateTest(ContainerFixture containerFixture)
    {
        _containerFixture = containerFixture;
    }

    [Fact]
    public async Task Should_Create()
    {
        // Arrange
        var sut = _containerFixture.GetService();
        var taskId = 777;
        var fileName = "file1.txt";
        // Act
        var actual = await sut.CreateInitialState(taskId, fileName);
        // Assert
        Assert.IsType<IProcessingStateService.State>(actual);
        Assert.Equal((int)taskId, (int)actual.TaskId);
        Assert.Equal(fileName, actual.FileName);

        var readResponse = await _containerFixture.Container.ReadItemAsync<az_appservice_dotnet.services.v1.CosmoDbProcessingStateService.CosmosState>(actual.Id,
            new PartitionKey(actual.TaskId));
        Assert.Equal(HttpStatusCode.OK, readResponse.StatusCode);
        var read = readResponse.Resource;
        Assert.IsType<az_appservice_dotnet.services.v1.CosmoDbProcessingStateService.CosmosState>(read);
        Assert.Equal(actual.Id, read.Id);
        Assert.Equal((int)actual.TaskId, read.TaskId);
        Assert.Equal(actual.Status, read.Status);
        Assert.Equal(actual.OriginalFileUrl, read.OriginalFileUrl);
        Assert.Equal(actual.ProcessedFileUrl, read.ProcessedFileUrl);
        Assert.Equal(actual.FileName, read.FileName);
    }
    
}