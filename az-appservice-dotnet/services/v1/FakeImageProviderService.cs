namespace az_appservice_dotnet.services.v1;

public class FakeImageProviderService : IImageProviderService
{
    public IImageProviderService.FileObject GetFileObject(string imageName, uint sizeInMb, IImageProviderService.FillType fillType = IImageProviderService.FillType.Random)
    {
        var tempFile = Path.GetTempFileName();
        // create an byte array of 1 MB
        var buffer = new byte[1024 * 1024];
        var tempFileStream = new FileStream(tempFile, FileMode.Append);

        if (fillType == IImageProviderService.FillType.Zero)
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

        return new IImageProviderService.FileObject(imageName, tempFile);
    }
}