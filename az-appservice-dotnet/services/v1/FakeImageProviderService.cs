namespace az_appservice_dotnet.services.v1;

public class FakeImageProviderService : IImageProviderService
{
    public enum FillType
    {
        Zero,
        Random
    }

    private readonly string _filePath;
    private readonly string _imageName;

    public FakeImageProviderService(string imageName,uint sizeInMb, FillType fillType = FillType.Random)
    {
        _imageName = imageName;
        var tempFile = Path.GetTempFileName();
        // create an byte array of 1 MB
        var buffer = new byte[1024 * 1024];
        var tempFileStream = new FileStream(tempFile, FileMode.Append);

        if (fillType == FillType.Zero)
        {
            for (var i = 0; i < sizeInMb; i++)
            {
                tempFileStream.Write(buffer, 0, buffer.Length);
            }
        }
        else
        {
            var random = new Random();
            for (var i = 0; i < sizeInMb; i++)
            {
                random.NextBytes(buffer);
                tempFileStream.Write(buffer, 0, buffer.Length);
            }
        }

        tempFileStream.Close();
        _filePath = tempFile;
    }

    public string GetFilePath()
    {
        return _filePath;
    }
    
    public string GetImageName()
    {
        return _imageName;
    }
}