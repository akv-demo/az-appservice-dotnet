namespace az_appservice_dotnet.services;

public interface IProcessingStateService
{
    public enum Status
    {
        Created,
        Uploading,
        WaitingForProcessing,
        Processing,
        Completed,
    }

    public struct StateId
    {
        private string _id;
        public static implicit operator StateId(string id) => new StateId { _id = id };
        public static implicit operator string(StateId stateId) => stateId._id;
    }

    public struct State
    {
        public readonly StateId Id;

        public readonly TaskId TaskId;

        public readonly Status Status;

        public readonly string? OriginalFileUrl;

        public readonly string? ProcessedFileUrl;

        public readonly string FileName;

        public State(string id, TaskId taskId, Status status, string? originalFileUrl, string? processedFileUrl,
            string fileName)
        {
            Id = id;
            TaskId = taskId;
            Status = status;
            OriginalFileUrl = originalFileUrl;
            ProcessedFileUrl = processedFileUrl;
            FileName = fileName;
        }
    }

    public ValueTask<State> CreateInitialState(TaskId taskId, string fileName);
    public ValueTask<StateId> MoveToUploadingStateAsync(TaskId id);
    public ValueTask<StateId> MoveToWaitingForProcessingStateAsync(TaskId id);
    public ValueTask<StateId> MoveToProcessingStateAsync(TaskId id);
    public ValueTask<StateId> MoveToCompletedStateAsync(TaskId id, string? processedFileUrl);
    public ValueTask<Dictionary<StateId, Status>> GetStatesDictionaryAsync();
}