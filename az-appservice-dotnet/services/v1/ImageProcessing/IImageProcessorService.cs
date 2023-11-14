namespace az_appservice_dotnet.services.v1.ImageProcessing;

public interface IImageProcessorService
{
    Task<string> ProcessImageAsync(string imageFilePath);
}