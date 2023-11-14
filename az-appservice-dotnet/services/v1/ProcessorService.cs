using az_appservice_dotnet.services.v1.Blob;
using az_appservice_dotnet.services.v1.ImageProcessing;
using az_appservice_dotnet.services.v1.State;

namespace az_appservice_dotnet.services.v1;

public class ProcessorService
{
    private IBlobService _blobService;
    private IProcessingStateService _processingStateService;
    private IImageProcessorService _imageProcessorService;
    private IImageProcessorService _nullImageProcessorService;

    public ProcessorService(IBlobService blobService, IProcessingStateService processingStateService,
        IImageProcessorService imageProcessorService, NullImageProcessorService nullImageProcessorService)
    {
        _imageProcessorService = imageProcessorService;
        _blobService = blobService;
        _processingStateService = processingStateService;
        _nullImageProcessorService = nullImageProcessorService;
    }

    public void StartWaitForImagesToProcess()
    {
        // TODO: see https://www.jetbrains.com/help/resharper/AsyncVoidLambda.html
        // ReSharper disable once AsyncVoidLambda
        _processingStateService.ListenToStateChanges(async state =>
        {
            if (state.Status == IProcessingStateService.Status.WaitingForProcessing)
            {
                Console.WriteLine(state.OriginalFileUrl);
                IImageProcessorService processor;
                if (_imageProcessorService.CanProcessImage(state.FileName))
                {
                    processor = _imageProcessorService;
                }
                else
                {
                    var supported = string.Join(", ", _imageProcessorService.SupportedFormats);
                    Console.WriteLine(
                        $"{state.FileName} is not supported (only {supported} are allowed).  Using NullImageProcessorService.");
                    processor = _nullImageProcessorService;
                }

                var stateProcessing = await _processingStateService.MoveToProcessingStateAsync(state);
                // add filename to keep extension
                var tmpFilePath = Path.GetTempFileName() + state.FileName;
                try
                {
                    await _blobService.DownloadBlobAsync(state.FileName, tmpFilePath);
                    var processedFilePath = await processor.ProcessImageAsync(tmpFilePath);
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