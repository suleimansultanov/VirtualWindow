using System.Linq;
using NasladdinPlace.Core.Models.Discounts;

namespace NasladdinPlace.Core.Repositories
{
    public interface IDiscountsRepository
    {
        IQueryable<Discount> GetAllIncludePosDiscounts();

        IQueryable<Discount> GetActiveDiscountsIncludeRulesByPosId(int posId);

        Discount GetByIdIncludePosDiscountsAndRules(int id);
        
    }
}
