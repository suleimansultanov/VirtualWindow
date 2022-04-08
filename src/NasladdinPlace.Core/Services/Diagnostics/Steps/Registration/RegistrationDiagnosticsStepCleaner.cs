using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories.UserNotification;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Cleaner;
using NasladdinPlace.Core.Services.Users.Account;
using NasladdinPlace.Core.Services.Users.Search.Model;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.Registration
{
    public class RegistrationDiagnosticsStepCleaner : IDiagnosticsStepCleaner
    {
        private readonly UserRegistrationInfo _userRegistrationInfo;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public RegistrationDiagnosticsStepCleaner(
            UserRegistrationInfo userRegistrationInfo, 
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (userRegistrationInfo == null)
                throw new ArgumentNullException(nameof(userRegistrationInfo));
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            
            _userRegistrationInfo = userRegistrationInfo;
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        
        public async Task CleanUpAsync(DiagnosticsContext context)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var users = await unitOfWork.Users.GetByFilterAsync(new Filter
                {
                    UserName = _userRegistrationInfo.PhoneNumber
                });

                if (!users.Any()) return;

                var user = users.First();

                await DeleteUserNotificationsAsync(unitOfWork, user.Id);
                await DeleteUserBonusPoints(unitOfWork, user.Id);
                await DeleteUserFirebaseTokensAsync(unitOfWork, user.Id);
                await DeleteUserPromotionLogsAsync(unitOfWork, user.Id);

                unitOfWork.Users.Remove(user.Id);
                
                await unitOfWork.CompleteAsync();
            }
        }

        private async Task DeleteUserNotificationsAsync(IUnitOfWork unitOfWork, int userId)
        {
            var userNotifications = await unitOfWork.UsersNotifications.GetByFilterAsync(new UserNotificationsFilter
            {
                UserId = userId
            });
                
            foreach (var userNotification in userNotifications)
            {
                unitOfWork.UsersNotifications.Remove(userNotification.Id);
            }
        }

        private async Task DeleteUserBonusPoints(IUnitOfWork unitOfWork, int userId)
        {
            var userBonusPoints = await unitOfWork.UsersBonusPoints.GetByUserAsync(userId);

            foreach (var bonusPoint in userBonusPoints)
            {
                unitOfWork.UsersBonusPoints.Remove(bonusPoint.Id);
            }
        }

        private async Task DeleteUserFirebaseTokensAsync(IUnitOfWork unitOfWork, int userId)
        {
            var userWithFirebaseTokens = await unitOfWork.Users.GetByIdIncludingPaymentCardsFirebaseTokensAndUserBonusPointsAsync(userId);
            var firebaseTokens = userWithFirebaseTokens.FirebaseTokens.ToImmutableList();

            var firebaseTokensRepository = unitOfWork.GetRepository<UserFirebaseToken>();
            
            foreach (var userFirebaseToken in firebaseTokens)
            {
                firebaseTokensRepository.Remove(userFirebaseToken.Id);
            }
        }

        private async Task DeleteUserPromotionLogsAsync(IUnitOfWork unitOfWork, int userId)
        {
            var user = await unitOfWork.Users.GetByIdIncludingPromotionLogsAsync(userId);
            var promotionLogs = user.PromotionLogs;

            var promotionLogRepository = unitOfWork.GetRepository<PromotionLog>();
            foreach (var promotionLog in promotionLogs)
            {
                promotionLogRepository.Remove(promotionLog.Id);
            }
        }
    }
}