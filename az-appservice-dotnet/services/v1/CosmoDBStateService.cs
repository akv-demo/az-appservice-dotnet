namespace az_appservice_dotnet.services.v1;

public class CosmoDbStateService: IStateService
{
    public ValueTask<IStateService.Status> GetStatusAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask SetStatus(IStateService.Status status)
    {
        throw new NotImplementedException();
    }
}