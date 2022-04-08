using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NasladdinPlace.Api.Services.FileStorage
{
    public interface IFileStorage
    {
        Task<FileStorageResult> SaveFile(
            IFormFile file, 
            string destinationFolder = "", 
            bool useFormFileName = false);
        Task<bool> DeleteFile(string fileRelativePath);
    }
}
