namespace az_appservice_dotnet.services.v1.UploadedFiles;

public class FakeFileProviderService : IFileProviderService
{
    public IFileProviderService.FileObject GetFileObject(string imageName, uint sizeInMb, IFileProviderService.FillType fillType = IFileProviderService.FillType.Random)
    {
        var tempFile = Path.GetTempFileName();
        // create an byte array of 1 MB
        var buffer = new byte[1024 * 1024];
        var tempFileStream = new FileStream(tempFile, FileMode.Append);

        if (fillType == IFileProviderService.FillType.Zero)
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

        return new IFileProviderService.FileObject(imageName, tempFile);
    }
}