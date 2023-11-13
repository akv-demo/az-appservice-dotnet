using az_appservice_dotnet.services.v1.Blob;
using az_appservice_dotnet.services.v1.State;
using az_appservice_dotnet.services.v1.UploadedFiles;

namespace az_appservice_dotnet.services.v1;

/**
 * <summary>
 * This is the Root service to upload the images to blobs and notify there are images
 * waiting to be processed
 *
 * It is not supposed to be mocked thus does not need to implement any interface
 * </summary>
 */
public class ProducerService
{
    private IBlobService _blobService;
    private IProcessingStateService _processingStateService;

    public ProducerService(IBlobService blobService, IProcessingStateService processingStateService)
    {
        _blobService = blobService;
        _processingStateService = processingStateService;
    }

    public void StartProcessImage(IFileProviderService.FileObject fileObject)
    {
        // TODO: won't it be GC'ed? ChatGPT said it won't, but need to be confirmed
        Task.Run(async () =>
        {
            var state1 = await _processingStateService.CreateInitialState(Task.CurrentId ?? -1, fileObject.Name);
            if (state1.TaskId == -1)
            {
                await _processingStateService.MoveToFailedStateAsync(state1, "No task id");
                return;
            }

            var state2 = await _processingStateService.MoveToUploadingStateAsync(state1);
            try
            {
                var blobUri = await _blobService.StoreBlobAsync(fileObject.Name, fileObject.Path);
                // TODO: not sure is it necessary to await the last call
                await _processingStateService.MoveToWaitingForProcessingStateAsync(state2, blobUri.ToString());
            }
            catch (Exception e)
            {
                // TODO: not sure is it necessary to await the last call
                await _processingStateService.MoveToFailedStateAsync(state2, e.Message);
            }
        }).ContinueWith(_ =>
        {
            if (File.Exists(fileObject.Path)) File.Delete(fileObject.Path);
        });
    }
}