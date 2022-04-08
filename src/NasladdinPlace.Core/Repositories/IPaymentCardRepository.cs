using NasladdinPlace.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Repositories
{
    public interface IPaymentCardRepository : IRepository<PaymentCard>
    {
        Task<List<PaymentCard>> GetAllByUserIdAsync(int userId);
        Task<PaymentCard> GetByIdAsync(int paymentCardId);
    }
}
