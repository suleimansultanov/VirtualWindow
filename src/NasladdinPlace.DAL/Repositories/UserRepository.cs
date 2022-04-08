using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using System.Linq.Dynamic.Core;
using System.Linq;
using System;
using System.Collections.Generic;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Users.Search.Model;
using NasladdinPlace.DAL.Utils;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.DAL.Repositories
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) 
            : base(context)
        {
        }

        public Task<List<ApplicationUser>> GetAllAsync()
        {
            return GetAll().ToListAsync();
        }

        public Task<List<ApplicationUser>> GetAllIncludingFirebaseTokensAsync()
        {
            return GetAll()
                .Include(u => u.FirebaseTokens)
                .ToListAsync();
        }

        public Task<ApplicationUser> GetByIdIncludingPaymentCardsFirebaseTokensAndUserBonusPointsAsync(int userId)
        {
            return GetAll()
                .Include(u => u.ActivePaymentCard)
                .Include(u => u.PaymentCards)
                .Include(u => u.FirebaseTokens)
                .Include(u => u.BonusPoints)
                .SingleOrDefaultAsync(u => u.Id == userId);
        }

        public Task<ApplicationUser> GetByIdIncludingPromotionLogsAsync(int userId)
        {
            return GetAll()
                .Include(u => u.PromotionLogs)
                .SingleOrDefaultAsync(u => u.Id == userId);
        }

        public Task<IOrderedQueryable<ApplicationUser>> GetByFilterAsync(Filter filter)
        {
            var query = GetAll()
                .Include(u => u.PosOperations)
                .AsQueryable();

            query = query
                .CheckAndFilter(!string.IsNullOrWhiteSpace(filter.UserName),
                    u => u.UserName != null && u.UserName.Contains(filter.UserName))
                .CheckAndFilter(!string.IsNullOrWhiteSpace(filter.Email),
                    u => u.Email != null && u.Email.Contains(filter.Email))
                .CheckAndFilter(!string.IsNullOrWhiteSpace(filter.PhoneNumber),
                    u => u.PhoneNumber != null && u.PhoneNumber.Contains(filter.PhoneNumber))
                .CheckAndFilter(filter.PhoneNumberConfirmed != null,
                    u => u.PhoneNumberConfirmed == filter.PhoneNumberConfirmed)
                .CheckAndFilter(filter.RegistrationInitiationDateRange.HasValue,
                    u => u.RegistrationInitiationDate >= filter.RegistrationInitiationDateRange.From &&
                         u.RegistrationInitiationDate <= filter.RegistrationInitiationDateRange.Until)
                .CheckAndFilter(filter.PaymentCardVerificationInitiationDateRange.HasValue,
                    u => u.PaymentCardVerificationInitiationDate != null &&
                         u.PaymentCardVerificationInitiationDate >= filter.PaymentCardVerificationInitiationDateRange.From &&
                         u.PaymentCardVerificationInitiationDate <= filter.PaymentCardVerificationInitiationDateRange.Until)
                .CheckAndFilter(filter.PaymentCardVerificationCompletionDateRange.HasValue,
                    u => u.PaymentCardVerificationCompletionDate != null &&
                         u.PaymentCardVerificationCompletionDate >= filter.PaymentCardVerificationCompletionDateRange.From &&
                         u.PaymentCardVerificationCompletionDate <= filter.PaymentCardVerificationCompletionDateRange.Until)
                .CheckAndFilter(filter.RegistrationCompletionDateRange.HasValue,
                    u => u.RegistrationCompletionDate != null &&
                         u.RegistrationCompletionDate >= filter.RegistrationCompletionDateRange.From &&
                         u.RegistrationCompletionDate <= filter.RegistrationCompletionDateRange.Until);

            query = query.Where(u => filter.Search == null || u.Email != null && u.Email.Contains(filter.Search) ||
                                     (u.PhoneNumber != null && u.PhoneNumber.Contains(filter.Search)) ||
                                     u.UserName != null && u.UserName.Contains(filter.Search));

            if (filter.SortBy == "DatePaid")
                filter.SortBy = "PosOperations.Max(DatePaid)";

            var orderedQuery = string.IsNullOrWhiteSpace(filter.SortBy)
                ? query.OrderBy(u => u.Id)
                : query.OrderBy(filter.SortBy);

            return Task.FromResult(orderedQuery);
        }

        public Task<List<ApplicationUser>> GetDebtorsAbleToPayAsync(TimeSpan considerAsDebtAfter)
        {
            var nowMinusDebtTime = DateTime.UtcNow.Add(-considerAsDebtAfter);
            return GetAll()
                .Where(u =>
                    u.ActivePaymentCardId != null &&
                    u.PosOperations.Any(po =>
                        po.DatePaid == null &&
                        po.CheckItems.Any(cki => cki.Status == CheckItemStatus.Unpaid) &&
                        po.DateStarted < nowMinusDebtTime
                    )
                )
                .ToListAsync();
        }

        public Task<List<ApplicationUser>> FindByPhoneNumberAsync(string phoneNumber)
        {
            return GetAll().Where(u => u.PhoneNumber == phoneNumber).ToListAsync();
        }

        public Task<List<ApplicationUser>> FindByFirebaseTokenAsync(string firebaseToken)
        {
            return GetAll()
                .Where(u => u.FirebaseTokens.Any(f => f.Token == firebaseToken))
                .ToListAsync();
        }

        public Task<ApplicationUser> GetByNameAsync(string userName)
        {
            return GetAll().FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public Task<List<ApplicationUser>> FindHavingBonusAmountExceedingAsync(decimal bonusAmount)
        {
            return GetAll()
                .Where(u => u.TotalBonusPoints > bonusAmount)
                .ToListAsync();
        }

        public IQueryable<ApplicationUser> GetAllIncludingPosOperationAndUserRoles()
        {
            return GetAll()
                .Include(u => u.PosOperations)
                .Include(u => u.UserRoles);
        }

        public Task<List<ApplicationUser>> GetNewInTimeRangeAsync(DateTimeRange reportDateTimeRange)
        {
            return GetAll()
                .Where(u => u.RegistrationInitiationDate >= reportDateTimeRange.Start &&
                            u.RegistrationInitiationDate <= reportDateTimeRange.End)
                .ToListAsync();
        }

        public Task<List<ApplicationUser>> GetLazyInDateRangeAsync(DateTimeRange reportDateTimePeriod, int daysCount)
        {
            return GetLazyUsersCountInDateTimePeriod(reportDateTimePeriod.End, daysCount)
                .Except(GetLazyUsersCountInDateTimePeriod(reportDateTimePeriod.Start, daysCount))
                .ToListAsync();
        }

        public Task<ApplicationUser> FindByIdIncludePaymentCardsAsync(int userId)
        {
            return GetAll()
                .Include(u => u.PaymentCards)
                .Include(u => u.ActivePaymentCard)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        private IQueryable<ApplicationUser> GetLazyUsersCountInDateTimePeriod(DateTime reportDateTime, int daysCount)
        {
            var finalDate = reportDateTime.Subtract(TimeSpan.FromDays(daysCount));
            return GetAll()
                .Where(u => u.PaymentCardVerificationCompletionDate != null && u.PosOperations == null &&
                            u.PaymentCardVerificationCompletionDate.Value <= reportDateTime && u.RegistrationCompletionDate != null &&
                            u.PaymentCardVerificationCompletionDate.Value > finalDate);
        }
    }
}