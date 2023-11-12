namespace az_appservice_dotnet.services;

public interface IStateService
{
    public enum Status
    {
        Uploading,
        WaitingForProcessing,
        Processing,
        Completed,
    }
    
    public ValueTask<Status> GetStatusAsync();
    public ValueTask SetStatus(Status status);
}