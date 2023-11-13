using System.Collections.Immutable;

namespace az_appservice_dotnet.services.v1.State.dependencies;

public interface IPersistProcessingStateProvider
{
    Task<IProcessingStateService.State> CreateStateAsync(in IProcessingStateService.State state);
    // it looks inconsistent to have id as input and does not seem to be useful 
    // Task<IProcessingStateService.State> ReadStateAsync(IProcessingStateService.StateId id);
    Task<IProcessingStateService.State> UpdateStateAsync(in IProcessingStateService.State state);
    Task<IProcessingStateService.State> DeleteStateAsync(in IProcessingStateService.State state);
    Task<ImmutableArray<IProcessingStateService.State>> ListStatesAsync();
}