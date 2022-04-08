namespace NasladdinPlace.Api.Services.Logs.User
{
    public interface IFolderAbsolutePathProvider
    {
        string Provide(string folderName);
    }
}