using Microsoft.AspNetCore.Http;
using NasladdinPlace.Utilities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Managers.PosScreenTemplates.Files.Contracts
{
    public interface IPosScreenTemplateFilesManager
    {
        string GetTemplateDirectoryPath(int templateId);
        string GetTemplateDirectoryAbsolutePath(int templateId);
        IReadOnlyCollection<string> GetMissingRequiredFileNamesForTemplate(int templateId);
        IReadOnlyCollection<string> GetTemplateFilesNames(int templateId);
        Task<Result> CreateTemplateDirectoryAsync(int templateId);
        Task<Result> DeleteTemplateDirectoryAsync(int templateId);
        Task<Result> DeleteTemplateFileAsync(int templateId, string fileName);
        Task<IReadOnlyCollection<Result>> UploadTemplateFilesAsync(int templateId, IEnumerable<IFormFile> files);
        Task<Result> UploadTemplateFileAsync(int templateId, IFormFile file);  
    }
}