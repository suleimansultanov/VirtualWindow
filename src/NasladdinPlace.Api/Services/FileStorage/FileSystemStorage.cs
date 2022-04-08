using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace NasladdinPlace.Api.Services.FileStorage
{
    public class FileSystemStorage : IFileStorage
    {
        private readonly IHostingEnvironment _environment;

        public FileSystemStorage(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<FileStorageResult> SaveFile(
            IFormFile file, 
            string destinationFolder = "",
            bool useFormFileName = false)
        {
            if (file == null || file.Length <= 0)
                return FileStorageResult.Failure();

            var folderAbsolutePath = CreateAbsolutePath(destinationFolder);

            Directory.CreateDirectory(folderAbsolutePath);

            var fileName = !useFormFileName 
                ? CreateUniqueFileName(file)
                : file.FileName;
            using (var fileStream = new FileStream(Path.Combine(folderAbsolutePath, fileName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                var relativeFilePath = $"{destinationFolder}/{fileName}";
                return FileStorageResult.Success(relativeFilePath);
            }
        }

        public Task<bool> DeleteFile(string fileRelativePath)
        {
            if (string.IsNullOrWhiteSpace(fileRelativePath))
                return Task.FromResult(false);

            fileRelativePath = fileRelativePath.StartsWith("/")
                ? fileRelativePath.Substring(1, fileRelativePath.Length - 1)
                : fileRelativePath;
            var fileAbsolutePath = CreateAbsolutePath(fileRelativePath);

            if (!File.Exists(fileAbsolutePath))
                return Task.FromResult(false);

            File.Delete(fileAbsolutePath);
            return Task.FromResult(true);
        }

        private string CreateAbsolutePath(string destinationFolderRelatievePath)
        {
            return Path.Combine(_environment.WebRootPath, destinationFolderRelatievePath);
        }

        private static string CreateUniqueFileName(IFormFile file)
        {
            return Guid.NewGuid() + file.FileName;
        }
    }
}
