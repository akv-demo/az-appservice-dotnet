namespace az_appservice_dotnet;

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