using System.Collections.Immutable;
using az_appservice_dotnet.services.v1.State.dependencies;

namespace az_appservice_dotnet.services.v1.State;

public class ProcessingStateService : IProcessingStateService
{
    private readonly IPublishProcessingStateProvider _publishProcessingStateProvider;
    private readonly IPersistProcessingStateProvider _persistProcessingStateProvider;

    public ProcessingStateService(
        IPublishProcessingStateProvider publishProcessingStateProvider,
        IPersistProcessingStateProvider persistProcessingStateProvider)
    {
        _publishProcessingStateProvider = publishProcessingStateProvider;
        _persistProcessingStateProvider = persistProcessingStateProvider;
    }

    private IProcessingStateService.State PublishTask(in Task<IProcessingStateService.State> task)
    {
        if (task.IsCompletedSuccessfully)
        {
            // Do not wait for the publish to complete
            _publishProcessingStateProvider.PublishStateAsync(task.Result);
        }

        return task.Result;
    }

    private Task<IProcessingStateService.State> UpdateStateAsync(in IProcessingStateService.State state)
    {
        return _persistProcessingStateProvider.UpdateStateAsync(state)
            .ContinueWith(task => PublishTask(task));
    }

    public Task<IProcessingStateService.State> CreateInitialState(in TaskId taskId, in string fileName)
    {
        return _persistProcessingStateProvider.CreateStateAsync(
            new IProcessingStateService.State(
                taskId,
                IProcessingStateService.Status.Created,
                null,
                null,
                fileName,
                null)
        ).ContinueWith(task => PublishTask(task));
    }

    public Task<IProcessingStateService.State> MoveToUploadingStateAsync(in IProcessingStateService.State state)
    {
        return UpdateStateAsync(state.WithStatus(IProcessingStateService.Status.Uploading));
    }

    public Task<IProcessingStateService.State> MoveToWaitingForProcessingStateAsync(
        in IProcessingStateService.State state, string originalFileUrl)
    {
        return UpdateStateAsync(state.WithWaitingForProcessingStatus(originalFileUrl));
    }

    public Task<IProcessingStateService.State> MoveToProcessingStateAsync(in IProcessingStateService.State state)
    {
        return UpdateStateAsync(state.WithStatus(IProcessingStateService.Status.Processing));
    }


    public Task<IProcessingStateService.State> MoveToCompletedStateAsync(in IProcessingStateService.State state,
        string processedFileUrl)
    {
        return UpdateStateAsync(state.WithCompletedStatus(processedFileUrl));
    }

    public Task<IProcessingStateService.State> MoveToFailedStateAsync(in IProcessingStateService.State state,
        string? failureReason)
    {
        return UpdateStateAsync(state.WithFailedStatus(failureReason));
    }

    public Task<ImmutableArray<IProcessingStateService.State>> GetStates()
    {
        return _persistProcessingStateProvider.ListStatesAsync();
    }
}