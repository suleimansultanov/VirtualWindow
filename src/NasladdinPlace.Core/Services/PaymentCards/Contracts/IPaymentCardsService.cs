using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.PaymentCards.Contracts
{
    public interface IPaymentCardsService
    {
        Task<ValueResult<List<PaymentCard>>> GetAllPaymentCardsAsync(int userId);
        Task<ValueResult<List<PaymentCard>>> DeletePaymentCardAsync(int userId, int paymentCardId);
        Task<ValueResult<List<PaymentCard>>> SetActivePaymentCardAsync(int userId, int paymentCardId);
        Task<ValueResult<PaymentCard>> GetActivePaymentCardAsync(int userId);
        Task<ValueResult<PaymentCard>> GetPaymentCardForPaymentAsync(int userId, int? paymentCardId);
        Task<Result> IsPaymentCardBelongsToTheUserAsync(int userId, int? paymentCardId);
    }
}
