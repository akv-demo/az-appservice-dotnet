using az_appservice_dotnet.services.v1.State;
using Azure.Messaging.ServiceBus;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.AzureSbSubscribeProcessingStateProvider;

[Collection("ServiceBusProcessor collection")]
public class AddStateChangeHandlerTest : Base
{
    public AddStateChangeHandlerTest(ServiceBusProcessorFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async void AddStateChangeHandler_ShouldHandleStateChange()
    {
        // Arrange
        List<IProcessingStateService.State> states = new();
        IProcessingStateService.StateChangeHandler handler =
            (in IProcessingStateService.State state) => { states.Add(state); };
        var provider = _fixture.GetProvider();
        provider.AddStateChangeHandler(handler);

        var state = await _fixture.PersistProcessingStateProvider()
            .CreateStateAsync(
                new IProcessingStateService.State(
                    1,
                    IProcessingStateService.Status.Created,
                    null,
                    null,
                    "file1.txt",
                    null));
        // remove possible outstaing messages
        Thread.Sleep(1000);
        states.Clear();
        // Act
        await _fixture.GetSender().SendMessageAsync(new ServiceBusMessage((string)state.Id + ":" + (int)state.TaskId));
        // Assert
        Thread.Sleep(1000);
        Assert.Single(states);
        Assert.Equal(state.Id, states.First().Id);
    }
}