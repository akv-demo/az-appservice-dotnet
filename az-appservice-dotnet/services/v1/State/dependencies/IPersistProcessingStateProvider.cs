using System.Collections.Immutable;

namespace az_appservice_dotnet.services.v1.State.dependencies;

public interface IPersistProcessingStateProvider
{
    Task<IProcessingStateService.State> CreateStateAsync(in IProcessingStateService.State state);
    // TODO: taskId is CosmosDb specific, user ListStatesAsync + filter instead
    //       it is ugly but therer's no other way to do it 
    //       - probably we can introduce abstract partition key instead of TaskId
    //         but there's a problem with it's type. 
    Task<IProcessingStateService.State> ReadStateAsync(IProcessingStateService.StateId id, TaskId taskId);
    
    Task<IProcessingStateService.State> UpdateStateAsync(in IProcessingStateService.State state);
    Task<IProcessingStateService.State> DeleteStateAsync(in IProcessingStateService.State state);
    Task<ImmutableDictionary<IProcessingStateService.StateId, IProcessingStateService.State>> ListStatesAsync();
}