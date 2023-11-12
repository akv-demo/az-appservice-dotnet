namespace az_appservice_dotnet.routing;

public static class RouterGroupBuilderPing
{
    public static RouteGroupBuilder MapPing(this RouteGroupBuilder group)
    {
        group.MapGet("/ping", () => "pong");
        return group;
    }
}