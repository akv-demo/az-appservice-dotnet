using System.Collections.Immutable;

namespace az_appservice_dotnet.services.v1.State;

public interface IProcessingStateService
{
    public enum Status
    {
        Created,
        Uploading,
        WaitingForProcessing,
        Processing,
        Completed,
        Failed,
    }

    public struct StateId
    {
        private string _id;
        public static implicit operator StateId(string id) => new() { _id = id };
        public static implicit operator string(StateId stateId) => stateId._id;
    }

    public readonly struct State
    {
        private readonly StateId? _idIncomplete;

        public readonly StateId Id => _idIncomplete ?? throw new InvalidOperationException();

        public readonly TaskId TaskId;

        public readonly Status Status;

        public readonly string? OriginalFileUrl;

        public readonly string? ProcessedFileUrl;

        public readonly string FileName;
        
        public readonly string? FailureReason;

        public State(TaskId taskId, Status status, string? originalFileUrl, string? processedFileUrl,
            string fileName, string? failureReason)
        {
            _idIncomplete = null;
            TaskId = taskId;
            Status = status;
            OriginalFileUrl = originalFileUrl;
            ProcessedFileUrl = processedFileUrl;
            FileName = fileName;
            FailureReason = failureReason;
        }
        
        public State(StateId id, TaskId taskId, Status status, string? originalFileUrl, string? processedFileUrl,
            string fileName, string? failureReason)
        {
            _idIncomplete = id;
            TaskId = taskId;
            Status = status;
            OriginalFileUrl = originalFileUrl;
            ProcessedFileUrl = processedFileUrl;
            FileName = fileName;
            FailureReason = failureReason;
        }

        public State(State state)
        {
            _idIncomplete = state._idIncomplete;
            TaskId = state.TaskId;
            Status = state.Status;
            OriginalFileUrl = state.OriginalFileUrl;
            ProcessedFileUrl = state.ProcessedFileUrl;
            FileName = state.FileName;
            FailureReason = state.FailureReason;
        }

        // I want to have it immutable, so have a method to add an Id to existing state
        // this is more fancy than useful
        public State AddId(StateId id)
        {
            return new State(id, TaskId, Status, OriginalFileUrl, ProcessedFileUrl, FileName, FailureReason);
        }
        
        public State WithStatus(Status status)
        {
            return new State(Id, TaskId, status, OriginalFileUrl, ProcessedFileUrl, FileName, FailureReason);
        }
        public State WithWaitingForProcessingStatus(string originalFileUrl)
        {
            return new State(Id, TaskId, Status.WaitingForProcessing, originalFileUrl, ProcessedFileUrl, FileName, FailureReason);
        }
        public State WithCompletedStatus(string processedFileUrl)
        {
            return new State(Id, TaskId, Status.Completed, OriginalFileUrl, processedFileUrl, FileName, FailureReason);
        }
        public State WithFailedStatus(string? failureReason)
        {
            return new State(Id, TaskId, Status.Failed, OriginalFileUrl, ProcessedFileUrl, FileName, failureReason);
        }
    }

    public Task<State> CreateInitialState(in TaskId taskId, in string fileName);
    public Task<State> MoveToUploadingStateAsync(in State state);
    public Task<State> MoveToWaitingForProcessingStateAsync(in State state, string originalFileUrl);
    public Task<State> MoveToProcessingStateAsync(in State state);
    public Task<State> MoveToCompletedStateAsync(in State state, string processedFileUrl);
    public Task<State> MoveToFailedStateAsync(in State state, string? failureReason);
    public Task<ImmutableArray<State>> GetStates();
}