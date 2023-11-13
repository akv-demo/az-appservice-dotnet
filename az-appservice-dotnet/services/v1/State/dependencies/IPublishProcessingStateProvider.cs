namespace az_appservice_dotnet.services.v1.State.dependencies;

public interface IPublishProcessingStateProvider
{
    Task<IProcessingStateService.State> PublishStateAsync(in IProcessingStateService.State state);
}