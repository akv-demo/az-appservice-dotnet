using az_appservice_dotnet.services.v1.State;

namespace az_appservice_dotnet.services.v1.Monitor;

public class ConsoleMonitor: IStateMonitor
{
    private readonly IProcessingStateService _processingStateService;
    
    public ConsoleMonitor(IProcessingStateService processingStateService)
    {
        _processingStateService = processingStateService;
    }
    
    public void StartStateMonitor()
    {
        _processingStateService.ListenToStateChanges(state =>
        {
            Console.WriteLine($"State changed to {state.Status}: file={state.FileName}, originalUrl={state.OriginalFileUrl}, processedUrl={state.ProcessedFileUrl}");
        });
    }
}