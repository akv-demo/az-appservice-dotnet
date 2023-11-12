namespace az_appservice_dotnet.routing.v1;

public static class RouteGroupBuilderBlobs
{
    public static RouteGroupBuilder MapBlobs(this RouteGroupBuilder group)
    {
        group.MapPost("/blobs", () =>
        {
            
        });
        return group;
    }
    
}