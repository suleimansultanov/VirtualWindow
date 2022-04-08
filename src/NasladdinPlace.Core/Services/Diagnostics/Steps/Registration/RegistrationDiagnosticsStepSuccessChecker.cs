using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories.UserNotification;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Models;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Assertion;
using NasladdinPlace.Core.Services.Users.Search.Model;
using NasladdinPlace.Utilities.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Users.Account;

namespace NasladdinPlace.Core.Services.Diagnostics.Steps.Registration
{
    public class RegistrationDiagnosticsStepSuccessChecker : IDiagnosticsStepSuccessChecker
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly UserRegistrationInfo _userRegistrationInfo;
        private readonly bool _shouldCheckForSendingSms;

        public RegistrationDiagnosticsStepSuccessChecker(
            IUnitOfWorkFactory unitOfWorkFactory, 
            UserRegistrationInfo userRegistrationInfo,
            bool shouldCheckForSendingSms)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (userRegistrationInfo == null)
                throw new ArgumentNullException(nameof(userRegistrationInfo));

            _unitOfWorkFactory = unitOfWorkFactory;
            _userRegistrationInfo = userRegistrationInfo;
            _shouldCheckForSendingSms = shouldCheckForSendingSms;
        }
        
        public async Task<Result> AssertStepExecutedSuccessfullyAsync(DiagnosticsContext context)
        {
            if (context.User == null)
                throw new InvalidOperationException(
                    $"Diagnostics context for {nameof(RegistrationDiagnosticsStepSuccessChecker)} must have a user.");

            if (!await AssertUserPersistedCorrectlyAsync())
            {
                return Result.Failure("A user has not been saved to persistence storage.");
            }
            
            if (_shouldCheckForSendingSms && !await AssertSmsMessageSentAsync(context.User))
            {
                return Result.Failure("Sms code has not been sent to a user.");
            }

            return Result.Success();
        }
        
        private async Task<bool> AssertUserPersistedCorrectlyAsync()
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var users = await unitOfWork.Users.GetByFilterAsync(new Filter
                {
                    PhoneNumber = _userRegistrationInfo.PhoneNumber
                });
                var userFromPersistenceStorage = users.FirstOrDefault();
                return userFromPersistenceStorage != null && 
                       userFromPersistenceStorage.PhoneNumber == _userRegistrationInfo.PhoneNumber &&
                       userFromPersistenceStorage.UserName == _userRegistrationInfo.PhoneNumber &&
                       userFromPersistenceStorage.PhoneNumberConfirmed;
            }
        }

        private async Task<bool> AssertSmsMessageSentAsync(ApplicationUser user)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var userNotifications = await unitOfWork.UsersNotifications.GetByFilterAsync(
                    new UserNotificationsFilter
                    {
                        UserId = user.Id,
                        NotificationType = NotificationType.Sms,
                        NotificationArea = NotificationArea.Registration
                    });
                
                return userNotifications.Any();
            }
        }
    }
}