namespace az_appservice_dotnet.services.v1.ImageProcessing;

public class NullImageProcessorService: IImageProcessorService
{
    public Task<string> ProcessImageAsync(string imageFilePath)
    {
        if (!File.Exists(imageFilePath))
        {
            throw new FileNotFoundException($"NullImageProcessorService:  File not found: {imageFilePath}");
        }

        return Task.Run(() =>
        {
            // get tmp file path
            var tmpFilePath = Path.GetTempFileName();
            // copy imageFilePath to tmpFilePath
            File.Copy(imageFilePath, tmpFilePath, true);
            return tmpFilePath;
        });
    }
}