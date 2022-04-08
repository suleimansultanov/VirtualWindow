using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Users.Search.Model;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Repositories
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<List<ApplicationUser>> GetAllAsync();
        Task<List<ApplicationUser>> GetAllIncludingFirebaseTokensAsync();
        Task<ApplicationUser> GetByIdIncludingPaymentCardsFirebaseTokensAndUserBonusPointsAsync(int userId);
        Task<ApplicationUser> GetByIdIncludingPromotionLogsAsync(int userId);
        Task<IOrderedQueryable<ApplicationUser>> GetByFilterAsync(Filter filter);
        Task<List<ApplicationUser>> GetDebtorsAbleToPayAsync(TimeSpan considerAsDebtAfter);
        Task<List<ApplicationUser>> GetLazyInDateRangeAsync(DateTimeRange reportDateTimePeriod, int daysCount);
        Task<List<ApplicationUser>> FindByPhoneNumberAsync(string phoneNumber);
        Task<List<ApplicationUser>> FindByFirebaseTokenAsync(string firebaseToken);
        Task<ApplicationUser> GetByNameAsync(string userName);
        Task<List<ApplicationUser>> FindHavingBonusAmountExceedingAsync(decimal bonusAmount);
        IQueryable<ApplicationUser> GetAllIncludingPosOperationAndUserRoles();
        Task<List<ApplicationUser>> GetNewInTimeRangeAsync(DateTimeRange reportDateTimeRange);
        Task<ApplicationUser> FindByIdIncludePaymentCardsAsync(int userId);
    }
}