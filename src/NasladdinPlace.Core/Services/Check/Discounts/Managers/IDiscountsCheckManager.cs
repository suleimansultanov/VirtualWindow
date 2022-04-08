using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Check.Discounts.Managers
{
    public interface IDiscountsCheckManager
    {
        Task AddDiscountsAsync(PosOperation posOperation, IUnitOfWork unitOfWork);
    }
}
