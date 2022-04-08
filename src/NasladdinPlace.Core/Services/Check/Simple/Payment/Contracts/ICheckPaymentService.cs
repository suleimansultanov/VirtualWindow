using NasladdinPlace.Core.Services.Check.Simple.Payment.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Check.Simple.Payment.Contracts
{
    public interface ICheckPaymentService
    {
        Task<CheckPaymentResult> PayForCheckAsync(IUnitOfWork unitOfWork, int userId, PaymentInfo paymentInfo);
    }
}