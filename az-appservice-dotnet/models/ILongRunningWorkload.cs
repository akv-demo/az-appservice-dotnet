namespace az_appservice_dotnet.models;

public interface ILongRunningWorkload
{
    void Next(uint seconds);
    uint Progress { get; }
}