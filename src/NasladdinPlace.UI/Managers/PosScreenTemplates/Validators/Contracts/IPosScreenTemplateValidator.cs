using NasladdinPlace.Utilities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Managers.PosScreenTemplates.Validators.Contracts
{
    public interface IPosScreenTemplateValidator
    {
        IReadOnlyCollection<Result> ValidateWhetherExists(int posScreenTemplateId);
        IReadOnlyCollection<Result> ValidateWhetherDefault(int posScreenTemplateId);
        Task<IReadOnlyCollection<Result>> ValidateWhetherExistsAsync(int posScreenTemplateId, string posScreenTemplateName);
        Task<IReadOnlyCollection<Result>> ValidateWhetherExistsAsync(string posScreenTemplateName);
    }
}