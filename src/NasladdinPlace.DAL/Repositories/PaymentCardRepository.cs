using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.DAL.Repositories
{
    public class PaymentCardRepository : Repository<PaymentCard>, IPaymentCardRepository
    {
        public PaymentCardRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<List<PaymentCard>> GetAllByUserIdAsync(int userId)
        {
            return GetAll()
                .Where(pc => pc.UserId == userId && pc.HasNumber &&
                             pc.Status == PaymentCardStatus.AbleToMakePayment)
                .OrderByDescending(pc => pc.CreatedDate)
                .Distinct()
                .ToListAsync();
        }

        public Task<PaymentCard> GetByIdAsync(int paymentCardId)
        {
            return GetAll()
                .FirstOrDefaultAsync(pc => pc.Id == paymentCardId);
        }
    }
}
