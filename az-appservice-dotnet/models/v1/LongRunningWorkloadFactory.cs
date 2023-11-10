namespace az_appservice_dotnet.models.v1;

public class LongRunningWorkloadFactory: ILongRunningWorkloadFactory
{
    public ILongRunningWorkload Create(uint duration)
    {
        return new LongRunningWorkloads(duration);
    }
}