namespace az_appservice_dotnet.models;

public interface ILongRunningWorkloadFactory
{
    ILongRunningWorkload Create(uint duration);
}