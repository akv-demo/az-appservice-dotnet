namespace az_appservice_dotnet.services.v1.State.dependencies;

public interface ISubscribeProcessingStateProvider
{
    void AddStateChangeHandler(IProcessingStateService.StateChangeHandler handler);
    void RemoveStateChangeHandler(IProcessingStateService.StateChangeHandler handler);
}