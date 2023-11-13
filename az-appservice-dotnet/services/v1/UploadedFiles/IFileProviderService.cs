namespace az_appservice_dotnet.services.v1.UploadedFiles;

public interface IFileProviderService
{
    public enum FillType
    {
        Zero,
        Random
    }
    
    public class FileObject
    {
       public readonly string Name;
       public readonly string Path;

       public FileObject(string name, string path)
       {
           Name = name;
           Path = path;
       }
    }
    FileObject GetFileObject(string imageName,uint sizeInMb, FillType fillType = FillType.Random);
}