using NasladdinPlace.Core.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.UserBalanceCalculator
{
    public interface IUserPaymentBalanceCalculator
    {
        Task<PaymentBalance> CalculateForUserAsync(int userId);
    }
}