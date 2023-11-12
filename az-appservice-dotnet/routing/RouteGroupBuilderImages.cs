using az_appservice_dotnet.services;

namespace az_appservice_dotnet;

public static class RouteGroupBuilderImages
{
    public static RouteGroupBuilder MapImages(this RouteGroupBuilder group, IImageProviderService? imageProviderService)
    {
        group.MapPost("/images", () =>
        {
            if (imageProviderService == null)
            {
                return Results.BadRequest("Image provider service is not configured");
            }
            var filePath = imageProviderService.GetFilePath();
            
        });
        return group;
    }
}