namespace az_appservice_dotnet.xUnit.providers.v1.Azure.AzureSbSubscribeProcessingStateProvider;

public class Base: IDisposable
{
    protected readonly ServiceBusProcessorFixture _fixture;
    protected readonly az_appservice_dotnet.providers.Azure.v1.AzureSbSubscribeProcessingStateProvider _provider;

    public Base(ServiceBusProcessorFixture fixture)
    {
        _fixture = fixture;
        _provider = _fixture.GetProvider();
    }

    public void Dispose()
    {
    }
}