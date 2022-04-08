namespace NasladdinPlace.Api.Services.FileStorage
{
    public class FileStorageResult
    {
        public bool Succeeded { get; set; }
        public string FileRelativePath { get; set; }

        private FileStorageResult()
        {
            FileRelativePath = string.Empty;
        }

        public static FileStorageResult Success(string imageRelativePath)
        {
            return new FileStorageResult
            {
                Succeeded = true,
                FileRelativePath = imageRelativePath
            };
        }

        public static FileStorageResult Failure()
        {
            return new FileStorageResult();
        }
    }
}
