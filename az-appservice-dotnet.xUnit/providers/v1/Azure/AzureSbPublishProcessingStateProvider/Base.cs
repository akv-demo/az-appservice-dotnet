namespace az_appservice_dotnet.xUnit.providers.v1.Azure.AzureSbPublishProcessingStateProvider;

public class Base: IDisposable
{
    protected readonly ServiceBusSenderFixture _fixture;
    protected readonly az_appservice_dotnet.providers.Azure.v1.AzureSbPublishProcessingStateProvider _provider;

    public Base(ServiceBusSenderFixture fixture)
    {
        _fixture = fixture;
        _provider = _fixture.GetProvider();
    }

    public void Dispose()
    {
    }
}