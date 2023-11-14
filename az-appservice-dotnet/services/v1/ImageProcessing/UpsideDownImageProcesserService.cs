namespace az_appservice_dotnet.services.v1.ImageProcessing;

public class UpsideDownImageProcessorService : IImageProcessorService
{
    public Task<string> ProcessImageAsync(string imageFilePath)
    {
        if (!File.Exists(imageFilePath))
        {
            throw new FileNotFoundException($"UpsideDownImageProcessorService:  File not found: {imageFilePath}");
        }

        var fileInfo = new FileInfo(imageFilePath);
        if (fileInfo.Length > 100 * 1024 * 1024)
        {
            throw new FileLoadException($"UpsideDownImageProcessorService:  File too large: {imageFilePath}");
        }

        return Task.Run(() =>
        {
            using (Image image = Image.Load(imageFilePath))
            {
                image.Mutate(x => x.Flip(FlipMode.Vertical));
                var tmpFilePath = Path.GetTempFileName() + ".jpg";
                image.SaveAsJpeg(tmpFilePath);
                return tmpFilePath;
            }
        });
    }

    public bool CanProcessImage(string imageFilePath)
    {
        return imageFilePath.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) ||
               imageFilePath.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase);
    }

    public string[] SupportedFormats
    {
        get { return new[] { "*.jpg", "*.png" }; }
    }
}