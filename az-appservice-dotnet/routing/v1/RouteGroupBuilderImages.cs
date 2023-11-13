using az_appservice_dotnet.services.v1;
using az_appservice_dotnet.services.v1.UploadedFiles;

namespace az_appservice_dotnet.routing.v1;

public static class RouteGroupBuilderImages
{
    public static RouteGroupBuilder MapImages(this RouteGroupBuilder group)
    {
        group.MapPost("/images", StartProcessingFromContext);
        group.MapGet("/images", StartProcessingFromFake);
        return group;
    }

    private static async Task<IResult> StartProcessingFromContext(HttpContext context, ProducerService producerService)
    {
        var formCollection = await context.Request.ReadFormAsync();
        var file = formCollection.Files.GetFile("file");
        if (file is not null && file.Length > 0)
        {
            var filePath = Path.GetTempFileName();

            await using (var stream = File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            producerService.StartProcessImage(new IFileProviderService.FileObject(file.FileName, filePath));
            return Results.Ok($"File '{file.FileName}' successfully queued.");
        }

        return Results.BadRequest("No file or empty file provided.");
    }

    private static IResult StartProcessingFromFake(HttpContext context, IFileProviderService fileProviderService,
        ProducerService producerService)
    {
        uint.TryParse(context.Request.Query["size"].ToString(), out var size);
        if (size == 0) size = 10;

        var name = context.Request.Query["name"].ToString();
        if (string.IsNullOrEmpty(name))
        {
            name = Guid.NewGuid() + ".bin";
        }

        var fileObject = fileProviderService.GetFileObject(name, size);
        producerService.StartProcessImage(fileObject);
        return Results.Ok($"File '{fileObject.Name}' successfully queued.");
    }
}