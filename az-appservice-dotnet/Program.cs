using System.Runtime.CompilerServices;
using az_appservice_dotnet.models;
using az_appservice_dotnet.models.v1;
using az_appservice_dotnet.routing;
using az_appservice_dotnet.services;
using az_appservice_dotnet.services.v1;

namespace az_appservice_dotnet;

static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSingleton<ILongRunningWorkloadFactory, LongRunningWorkloadFactory>();
        builder.Services.AddSingleton<ILongRunningTasksService, LongRunningTasksService>();
        builder.Services.AddSingleton<IImageProviderService, FakeImageProviderService>();
        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");
        app.MapGroup("/1")
            .MapApi1(app);

        app.Run();
    }

    static RouteGroupBuilder MapApi1(this RouteGroupBuilder group, WebApplication app)
    {
        group.MapPing();
        group.MapBlobs();
        group.MapImages(app.Services.GetService<IImageProviderService>());
        return group;
    }
}