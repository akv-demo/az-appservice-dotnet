using az_appservice_dotnet.models;
using az_appservice_dotnet.models.v1;
using az_appservice_dotnet.routing.v1;
using az_appservice_dotnet.services;
using az_appservice_dotnet.services.v1;

namespace az_appservice_dotnet;

static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<ILongRunningWorkloadFactory, LongRunningWorkloadFactory>();
        builder.Services.AddSingleton<ILongRunningTasksService, LongRunningTasksService>();
        builder.Services.AddSingleton<IImageProviderService, FakeImageProviderService>();
        builder.Services.AddSingleton<IProcessingStateService, CosmoDbProcessingStateService>();
        var app = builder.Build();

        app.MapGet("/", () => Results.Ok("Hello World!"));
        app.MapGroup("/1")
            .MapApi1(app);

        await app.RunAsync();
    }

    static RouteGroupBuilder MapApi1(this RouteGroupBuilder group, WebApplication app)
    {
        group.MapPing();
        group.MapBlobs();
        group.MapImages();
        return group;
    }
}