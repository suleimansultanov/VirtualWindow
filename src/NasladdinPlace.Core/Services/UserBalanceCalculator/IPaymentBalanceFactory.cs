using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Simple.Models;

namespace NasladdinPlace.Core.Services.UserBalanceCalculator
{
    public interface IPaymentBalanceFactory
    {
        PaymentBalance Create(SimpleCheck check, int userId);
    }
}