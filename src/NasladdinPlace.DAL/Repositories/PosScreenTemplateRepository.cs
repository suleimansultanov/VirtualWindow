using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.DAL.Repositories
{
    public class PosScreenTemplateRepository : Repository<PosScreenTemplate>, IPosScreenTemplateRepository
    {
        public PosScreenTemplateRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<PosScreenTemplate> GetIncludingPointsOfSaleAsync(int templateId)
        {
            return GetAll()
                .Include(pst => pst.PointsOfSale)
                .SingleOrDefaultAsync(pst => pst.Id == templateId);
        }

        public IQueryable<PosScreenTemplate> GetAllIncludingPointsOfSaleOrderedByName()
        {
            return GetAll()
                .Include(pst => pst.PointsOfSale)
                .OrderBy(pst => pst.Name);
        }

        public Task<PosScreenTemplate> GetByNameAsync(string templateName)
        {
            return GetAll()
                .Include(pst => pst.PointsOfSale)
                .FirstOrDefaultAsync(pst => pst.Name == templateName);
        }
    }
}