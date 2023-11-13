using az_appservice_dotnet.services.v1.State;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.AzureSbPublishProcessingStateProvider;

[Collection("ServiceBusSender collection")]
public class PublishStateAsyncTest : Base
{
    public PublishStateAsyncTest(ServiceBusSenderFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Should_PublishState()
    {
        // Arrange
        var (messages, errors,_) = _fixture.RunProcessor();
        var state = new IProcessingStateService.State(
            "some-id-to-be-published",
            1,
            IProcessingStateService.Status.Created,
            null,
            null,
            "file1.txt",
            null);
        // Act
        var result = await _provider.PublishStateAsync(state);
        // TODO: Find a better way to wait for the message to be processed; semaphore?
        Thread.Sleep(1000);
        // Assert
        Assert.Equal(state, result);
        Assert.Empty(errors);
        Assert.Single(messages);
        var message = messages.First().Message.Body.ToString();
        Assert.Equal(state.Id, message);
    }
}