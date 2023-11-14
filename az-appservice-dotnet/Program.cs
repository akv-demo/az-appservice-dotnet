using az_appservice_dotnet.providers.Azure.v1;
using az_appservice_dotnet.routing.v1;
using az_appservice_dotnet.services;
using az_appservice_dotnet.services.v1;
using az_appservice_dotnet.services.v1.Blob;
using az_appservice_dotnet.services.v1.Blob.dependencies;
using az_appservice_dotnet.services.v1.ImageProcessing;
using az_appservice_dotnet.services.v1.Monitor;
using az_appservice_dotnet.services.v1.State;
using az_appservice_dotnet.services.v1.State.dependencies;
using az_appservice_dotnet.services.v1.UploadedFiles;

namespace az_appservice_dotnet;

static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddSingleton<IBlobProvider, AzureBlobProvider>();
        builder.Services.AddSingleton<IPersistProcessingStateProvider, CosmosDbPersistProcessingStateProvider>();
        builder.Services.AddSingleton<IPublishProcessingStateProvider, AzureSbPublishProcessingStateProvider>();
        builder.Services.AddSingleton<ISubscribeProcessingStateProvider, AzureSbSubscribeProcessingStateProvider>();
        
        builder.Services.AddSingleton<IFileProviderService, FakeFileProviderService>();
        builder.Services.AddSingleton<IBlobService, BlobService>();
        builder.Services.AddSingleton<IProcessingStateService, ProcessingStateService>();
        builder.Services.AddSingleton<IStateMonitor, ConsoleMonitor>();
        builder.Services.AddSingleton<IImageProcessorService, NullImageProcessorService>();
        
        builder.Services.AddSingleton<ProducerService>();
        builder.Services.AddSingleton<ProcessorService>();
        
        var app = builder.Build();
        
        // CheckProvidersHealth(app);
        
        app.MapGet("/", () =>
        {
            return Results.Redirect("/1/ping?workload=pong");
        });
        
        app.MapGroup("/1")
            .MapApi1(app);
        
        var monitorService = app.Services.GetService<IStateMonitor>();
        if (monitorService == null)
        {
            throw new Exception("StateMonitor is not registered");
        }
        monitorService.StartStateMonitor();

        var processorService = app.Services.GetService<ProcessorService>();
        if (processorService == null)
        {
            throw new Exception("ProcessorService is not registered");
        }
        processorService.StartWaitForImagesToProcess();
        await app.RunAsync();
    }

    static RouteGroupBuilder MapApi1(this RouteGroupBuilder group, WebApplication app)
    {
        group.MapPing();
        group.MapImages();
        return group;
    }
    
    private static void CheckProvidersHealth(WebApplication app)
    {
        var services = app.Services;
        services.GetRequiredService<IBlobProvider>();
        services.GetRequiredService<IPersistProcessingStateProvider>();
        services.GetRequiredService<IPublishProcessingStateProvider>();
    }
}