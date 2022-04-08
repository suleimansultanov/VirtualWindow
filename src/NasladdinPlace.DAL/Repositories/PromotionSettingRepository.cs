using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.DAL.Contracts;

namespace NasladdinPlace.DAL.Repositories
{
    public class PromotionSettingRepository: IPromotionSettingRepository
    {
        private readonly IApplicationDbContext _context;

        public PromotionSettingRepository(IApplicationDbContext context)
        {
            _context = context;
        }
        
        public Task<PromotionSetting> GetByPromotionTypeAsync(PromotionType promotionType)
        {
            return _context.PromotionSettings.SingleOrDefaultAsync(p => p.PromotionType == promotionType);
        }
    }
}
