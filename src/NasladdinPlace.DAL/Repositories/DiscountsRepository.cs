using System.Linq;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.Core.Repositories;

namespace NasladdinPlace.DAL.Repositories
{
    public class DiscountsRepository : IDiscountsRepository
    {
        private readonly ApplicationDbContext _context;

        public DiscountsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Discount> GetAllIncludePosDiscounts()
        {
            return _context.Discounts
                           .Include(d => d.PosDiscounts)
                           .ThenInclude(c => c.Pos);
        }

        public IQueryable<Discount> GetActiveDiscountsIncludeRulesByPosId(int posId)
        {
            return _context.Discounts
                           .Include(d => d.PosDiscounts)
                           .Include(d => d.DiscountRules)
                           .ThenInclude(d => d.DiscountRuleValues)
                           .Where(d => d.IsEnabled &&  
                                       d.PosDiscounts.Any(p => p.PosId == posId));
        }

        public Discount GetByIdIncludePosDiscountsAndRules(int id)
        {
            return _context.Discounts
                           .Include(d => d.PosDiscounts)
                           .ThenInclude(c => c.Pos)
                           .Include(d => d.DiscountRules)
                           .ThenInclude(d => d.DiscountRuleValues)
                           .FirstOrDefault(d => d.Id == id);
        }
    }
}
