using Microsoft.AspNetCore.Http;
using NasladdinPlace.UI.ViewModels.PointsOfSale;
using NasladdinPlace.Utilities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Managers.PosScreenTemplates
{
    public interface IPosScreenTemplateManager
    {
        Task<Result> CreatePosScreenTemplateAsync(string posScreenTemplateName);
        Task<Result> EditPosScreenTemplateAsync(int posScreenTemplateId, string posScreenTemplateName, IEnumerable<PosBasicInfoViewModel> pointsOfSale);
        Task<Result> DeletePosScreenTemplateAsync(int posScreenTemplateId);
        Task<Result> UploadTemplateFileAsync(int posScreenTemplateId, IFormFile file);
        Task<Result> DeleteTemplateFileAsync(int posScreenTemplateId, string fileName);
    }
}