using az_appservice_dotnet.providers.Azure.v1;

namespace az_appservice_dotnet.xUnit.providers.v1.Azure.CosmoDbPersistProcessingStateProvider;

public class Base : IDisposable
{
    private string _name;

    protected readonly ContainerFixture _containerFixture;
    protected readonly CosmosDbPersistProcessingStateProvider _provider;

    protected Base(string name, ContainerFixture containerFixture)
    {
        _name = name;
        _containerFixture = containerFixture;
        _provider = containerFixture.GetProvider(name + "_" + Guid.NewGuid().ToString("N"));
        containerFixture.Clear(_provider);
    }

    public void Dispose()
    {
        _containerFixture.Clear(_provider, true);
    }
}