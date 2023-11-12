namespace az_appservice_dotnet.services;

public interface IImageProviderService
{
    String GetFilePath();
    String GetImageName();
}