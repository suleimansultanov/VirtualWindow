using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Discounts;

namespace NasladdinPlace.Core.Services.Check.Discounts.Rules
{
    public interface IRuleChecker
    {
        Task<bool> IsMatchedAsync(DiscountRule rule, PosOperation posOperation, IUnitOfWork unitOfWork = null);
    }
}
