using az_appservice_dotnet.services.v1.Blob;
using az_appservice_dotnet.services.v1.ImageProcessing;
using az_appservice_dotnet.services.v1.State;

namespace az_appservice_dotnet.services.v1;

public class ProcessorService
{
    private IBlobService _blobService;
    private IProcessingStateService _processingStateService;
    private IImageProcessorService _imageProcessorService;

    public ProcessorService(IBlobService blobService, IProcessingStateService processingStateService,
        IImageProcessorService imageProcessorService)
    {
        _imageProcessorService = imageProcessorService;
        _blobService = blobService;
        _processingStateService = processingStateService;
    }

    public void StartWaitForImagesToProcess()
    {
        _processingStateService.ListenToStateChanges(async state =>
        {
            if (state.Status == IProcessingStateService.Status.WaitingForProcessing)
            {
                Console.WriteLine(state.OriginalFileUrl);
                var tmpFilePath = Path.GetTempFileName();
                var stateProcessing = await _processingStateService.MoveToProcessingStateAsync(state);
                try
                {
                    var ok = await _blobService.DownloadBlobAsync(state.FileName, tmpFilePath);
                    var processedFilePath = await _imageProcessorService.ProcessImageAsync(tmpFilePath);
                    var uploadedUri =
                        await _blobService.UploadBlobAsync($"processed-{state.FileName}", processedFilePath);
                    await _processingStateService.MoveToCompletedStateAsync(stateProcessing, uploadedUri.ToString());
                }
                catch (Exception e)
                {
                    await _processingStateService.MoveToFailedStateAsync(stateProcessing, e.Message);
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    if (File.Exists(tmpFilePath))
                    {
                        File.Delete(tmpFilePath);
                    }
                }
            }
        });
    }
}