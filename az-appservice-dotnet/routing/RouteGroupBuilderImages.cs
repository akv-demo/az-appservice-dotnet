namespace az_appservice_dotnet;

public static class RouteGroupBuilderImages
{
    public static RouteGroupBuilder MapImages(this RouteGroupBuilder group)
    {
        group.MapPost("/images", () =>
        {
            
        });
        return group;
    }
}