using az_appservice_dotnet.models;
using az_appservice_dotnet.models.v1;
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
        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");
        app.MapGroup("/1")
            .MapApi1();

        app.Run();
    }

    static RouteGroupBuilder MapApi1(this RouteGroupBuilder group)
    {
        group.MapGet("/ping", () => "pong");
        group.MapBlobs();
        group.MapImages();
        return group;
    }
}