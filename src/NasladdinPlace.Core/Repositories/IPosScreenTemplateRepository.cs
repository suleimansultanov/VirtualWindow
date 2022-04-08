using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface IPosScreenTemplateRepository : IRepository<PosScreenTemplate>
    {
        Task<PosScreenTemplate> GetIncludingPointsOfSaleAsync(int templateId);
        IQueryable<PosScreenTemplate> GetAllIncludingPointsOfSaleOrderedByName();
        Task<PosScreenTemplate> GetByNameAsync(string templateName);
    }
}