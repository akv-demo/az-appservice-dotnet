using az_appservice_dotnet.services;

namespace az_appservice_dotnet.routing.v1;

public static class RouteGroupBuilderImages
{
    public static RouteGroupBuilder MapImages(this RouteGroupBuilder group)
    {
        group.MapPost("/images", Create);
        group.MapGet("/images", Create);
        return group;
    }

    private static IResult Create(IImageProviderService imageProviderService)
    {
        var file = imageProviderService.GetFileObject("image", 1);

        return TypedResults.Created($"/images/{file.Name}", new { Name = file.Name });
    }
}