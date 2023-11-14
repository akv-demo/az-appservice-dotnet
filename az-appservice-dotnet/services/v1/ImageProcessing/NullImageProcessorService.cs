namespace az_appservice_dotnet.services.v1.ImageProcessing;

public class NullImageProcessorService : IImageProcessorService
{
    public Task<string> ProcessImageAsync(string imageFilePath)
    {
        if (!File.Exists(imageFilePath))
        {
            throw new FileNotFoundException($"NullImageProcessorService:  File not found: {imageFilePath}");
        }

        var fileInfo = new FileInfo(imageFilePath);
        if (fileInfo.Length > 100 * 1024 * 1024)
        {
            throw new FileLoadException($"NullImageProcessorService:  File too large: {imageFilePath}");
        }

        return Task.Run(() =>
        {
            var tmpFilePath = Path.GetTempFileName();
            File.Copy(imageFilePath, tmpFilePath, true);
            return tmpFilePath;
        });
    }

    public bool CanProcessImage(string imageFilePath)
    {
        return true;
    }

    public string[] SupportedFormats
    {
        get { return new[] { "*" }; }
    }
}