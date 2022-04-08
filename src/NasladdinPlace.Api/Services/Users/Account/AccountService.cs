using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Resources.AccountService;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.MessageSender.Sms.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Sms.Models;
using NasladdinPlace.Core.Services.Users.Account;
using NasladdinPlace.Core.Services.Users.Manager;
using NasladdinPlace.Core.Services.Users.Test;
using NasladdinPlace.Utilities.DateTimeConverter.Extensions;
using NasladdinPlace.Utilities.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.Users.Account
{
    public class AccountService : IAccountService
    {
        private readonly IUserManager _userManager;
        private readonly ISmsSender _smsSender;
        private readonly ITestUserInfoProvider _testUserInfoProvider;
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _environment;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IUnitOfWork _unitOfWork;

        private const string TestPhoneNubmer = "79857125410";

        public AccountService(IServiceProvider serviceProvider)
        {
            _userManager = serviceProvider.GetRequiredService<IUserManager>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _testUserInfoProvider = serviceProvider.GetRequiredService<ITestUserInfoProvider>();
            _environment = serviceProvider.GetRequiredService<IHostingEnvironment>();
            _smsSender = serviceProvider.GetRequiredService<ISmsSender>();
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        }

        public async Task<ValueResult<UnverifiedUserInfo>> RegisterUserAsync(UserRegistrationInfo userRegistrationInfo)
        {
            var phoneNumber = userRegistrationInfo.PhoneNumber;

            var user = await _userManager.FindByNameAsync(phoneNumber);

            if (user == null)
            {
                user = await TryCreateUserAsync(userRegistrationInfo);

                if (user == null)
                    return ValueResult<UnverifiedUserInfo>.Failure($"Cannot create a user with {phoneNumber}.");

                LogInformation($"User with phone number {userRegistrationInfo.PhoneNumber} created a new account.");
            }
            else
                LogInformation($"A user with the phone number {phoneNumber} already has an account.");

            if (IsTestUser(user))
                return ValueResult<UnverifiedUserInfo>.Success(new UnverifiedUserInfo(user, "1234"));

            var verificationCode = await GenerateVerificationCodeAsync(user, phoneNumber);
            var shortedVerificationCodeResult =
                await MakeShortenVerificationCodeAndUpdateUserAsync(user, verificationCode);

            if (!shortedVerificationCodeResult.Succeeded)
                return ValueResult<UnverifiedUserInfo>.Failure(
                    $"Some error has been occured during shortening user {phoneNumber} verification code. {shortedVerificationCodeResult.Error}.");

            verificationCode = shortedVerificationCodeResult.Value;

            if (await SendVerificationCodeAsync(phoneNumber, verificationCode))
                return ValueResult<UnverifiedUserInfo>.Success(new UnverifiedUserInfo(user, verificationCode));

            LogError($"Some error has been occurred during sending sms to {phoneNumber}. Try again later.");

            var smsNotSendedError =
                AccountServiceResource.ResourceManager.GetString(
                    "Some error has been occurred during sending sms to {0}. Try again later.");
            return ValueResult<UnverifiedUserInfo>.Failure(string.Format(smsNotSendedError, phoneNumber));
        }

        public async Task<ValueResult<VerifiedUserInfo>> VerifyPhoneNumberAsync(VerificationPhoneNumberInfo verificationPhoneNumberInfo)
        {
            var phoneNumber = verificationPhoneNumberInfo.PhoneNumber;
            var code = verificationPhoneNumberInfo.VerificationCode;

            var user = await _userManager.FindByNameAsync(phoneNumber);

            if (user == null)
                return ValueResult<VerifiedUserInfo>.Failure($"The user with phone number {phoneNumber} does not exist.");

            var verifiedUserInfo = new VerifiedUserInfo(user, generatedPassword: Guid.NewGuid().ToString());

            if (user.ActivePaymentCardId.HasValue)
                verifiedUserInfo.PreviousRegistrationStatus = UserRegistrationStatus.BankingCardConfirmed;

            else if (user.RegistrationCompletionDate.HasValue)
                verifiedUserInfo.PreviousRegistrationStatus = UserRegistrationStatus.PhoneNumberConfirmed;

            else
            {
                user.NotifyRegistrationCompletion();
                verifiedUserInfo.PreviousRegistrationStatus = UserRegistrationStatus.None;
            }

            var result = await _userManager.ChangePhoneNumberAsync(user, phoneNumber, user.GetFullChangePhoneNumberToken(code));

            if (!IsTestUser(user) && !result.Succeeded)
                return ValueResult<VerifiedUserInfo>.Failure(string.Join(". ", result.Errors));

            user.PhoneNumberConfirmed = true;
            var updateUserResult = await _userManager.UpdateAsync(user);

            if (!updateUserResult.Succeeded)
            {
                LogError(
                    $"Some error has been occured during verify of a phone number of a user {user.Id}.");

                return ValueResult<VerifiedUserInfo>.Failure(string.Join(". ", updateUserResult.Errors));
            }

            LogInformation($"The verification of phone number {phoneNumber} is successfully completed.");
            if (!await TryChangeUserPasswordAsync(user, verifiedUserInfo.GeneratedPassword))
                return ValueResult<VerifiedUserInfo>.Failure($"Unable to generate user {phoneNumber} password.");

            await AddBonusesToUserAsync(user);

            return ValueResult<VerifiedUserInfo>.Success(verifiedUserInfo);
        }

        public async Task<Result> SendVerificationCodeAsync(int userId, string phoneNumber)
        {
            var user = await FindUserByIdAsync(userId);

            if (user == null)
                return Result.Failure($"A user with the phone number {phoneNumber} does not exist.");

            if (user.PhoneNumber == phoneNumber && TestPhoneNubmer != phoneNumber)
            {
                var numberAlreadyUsedByUser =
                    AccountServiceResource.ResourceManager.GetString("The number is already used by you.");
                return Result.Failure(numberAlreadyUsedByUser);
            }

            var verificationCode = await GenerateVerificationCodeAsync(user, phoneNumber);
            var shortedVerificationCodeResult =
                await MakeShortenVerificationCodeAndUpdateUserAsync(user, verificationCode);

            if (!shortedVerificationCodeResult.Succeeded)
                return Result.Failure(
                    $"Some error has been occured during shortening user {phoneNumber} verification code. {shortedVerificationCodeResult.Error}.");

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                verificationCode = shortedVerificationCodeResult.Value;

            if (IsTestUser(user) || await SendVerificationCodeAsync(phoneNumber, verificationCode, userId))
                return Result.Success();

            LogError($"Some error has been occurred during sending sms to {phoneNumber} for user {user.Id}.");

            return Result.Failure($"Some error has been occured during sending of a verification code");
        }

        public async Task<Result> ChangePhoneNumberAsync(int userId, ChangePhoneNumberInfo changePhoneNumberInfo)
        {
            var user = await FindUserByIdAsync(userId);

            if (IsTestUser(user))
                return Result.Success();

            var changePhoneNumberToken = user.GetFullChangePhoneNumberToken(changePhoneNumberInfo.VerificationCode);
            var changedPhoneNumber = changePhoneNumberInfo.PhoneNumber;
            var result = await _userManager.ChangePhoneNumberAsync(
                user, changedPhoneNumber, changePhoneNumberToken);

            if (!result.Succeeded)
            {
                var wrongCodeError = AccountServiceResource.ResourceManager.GetString("Wrong verification code.");
                return Result.Failure(wrongCodeError);
            }

            user.UserName = changedPhoneNumber;
            user.PhoneNumber = changedPhoneNumber;
            user.PhoneNumberConfirmed = true;

            var updateUserResult = await _userManager.UpdateAsync(user);

            if (!updateUserResult.Succeeded)
            {
                LogError(
                    $"Some error has been occured during changing of a phone number of a user {user.Id} to {changedPhoneNumber}."
                );

                return Result.Failure(string.Join(". ", updateUserResult.Errors));
            }

            LogInformation(
                $"User's {user.Id} phone number has been successfully changed to {changedPhoneNumber}."
            );

            return Result.Success();
        }

        public async Task<Result> PatchUserAsync(UserInfo userInfo)
        {
            var user = await FindUserByIdAsync(userInfo.UserId);

            if (!string.IsNullOrWhiteSpace(userInfo.Email))
                user.Email = userInfo.Email;

            if (!string.IsNullOrWhiteSpace(userInfo.FullName))
                user.FullName = userInfo.FullName;

            if (userInfo.Gender.HasValue && Enum.IsDefined(typeof(Gender), userInfo.Gender.Value))
                user.Gender = (Gender)userInfo.Gender.Value;

            if (userInfo.BirthDateMilliseconds.HasValue)
            {
                var birthDate = userInfo.BirthDateMilliseconds.Value.ToDateTimeSince1970();
                user.Birthdate = birthDate;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                LogInformation(
                    $"User info {user.Id} has been successfully updated."
                );

                return Result.Success();
            }

            var error = $"Some error has been occured during update of user info a user {user.Id}: " +
                        $"{JsonConvert.SerializeObject(result.Errors)}";

            LogError(error);

            return Result.Failure(error);
        }

        public async Task<ValueResult<ApplicationUser>> GetUserAccountAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdIncludingPaymentCardsFirebaseTokensAndUserBonusPointsAsync(userId);

            return user == null
                ? ValueResult<ApplicationUser>.Failure("User not exists.")
                : ValueResult<ApplicationUser>.Success(user);
        }

        public async Task<Result> UpdateFirebaseTokenAsync(int userId, string token, Core.Enums.Brand brand)
        {
            var user = await _unitOfWork.Users.GetByIdIncludingPaymentCardsFirebaseTokensAndUserBonusPointsAsync(userId);

            if (user == null)
                return Result.Failure("User is not exists.");

            user.CreateOrUpdateFirebaseToken(brand, token);

            await _unitOfWork.CompleteAsync();

            return Result.Success();
        }

        private async Task AddBonusesToUserAsync(ApplicationUser user)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var promotionSetting =
                    await unitOfWork.PromotionSettings.GetByPromotionTypeAsync(PromotionType.VerifyPhoneNumber);
                if (promotionSetting != null && promotionSetting.IsEnabled)
                {
                    var userBonusPoints = await unitOfWork.UsersBonusPoints.GetByUserAsync(user.Id);

                    if (!userBonusPoints.Any())
                    {
                        unitOfWork.Users.Update(user);
                        user.AddBonusPoints(promotionSetting.BonusValue, BonusType.VerifyPhoneNumber);
                        await unitOfWork.CompleteAsync();
                    }
                }
            }
        }

        #region Helpers
        private async Task<ApplicationUser> TryCreateUserAsync(UserRegistrationInfo userRegistrationInfo)
        {
            var user = new ApplicationUser
            {
                UserName = userRegistrationInfo.PhoneNumber,
                PhoneNumber = userRegistrationInfo.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user);

            return result.Succeeded ? user : null;
        }

        private async Task<bool> TryChangeUserPasswordAsync(ApplicationUser user, string password)
        {
            var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordToken, password);
            return result.Succeeded;
        }

        private Task<string> GenerateVerificationCodeAsync(ApplicationUser user, string phoneNumber)
        {
            return _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);
        }

        private async Task<ValueResult<string>> MakeShortenVerificationCodeAndUpdateUserAsync(
            ApplicationUser user, string fullVerificationCode)
        {
            var shortCode = user.ShortenChangePhoneTokenAndSaveRemainder(fullVerificationCode);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? ValueResult<string>.Success(shortCode) :
                ValueResult<string>.Failure(string.Join(". ", result.Errors));
        }

        private async Task<bool> SendVerificationCodeAsync(string phoneNumber, string code, int? userId = null)
        {
            LogInformation($"Verification code for phone number {phoneNumber} is {code}.");

            var message = $"Your verification code: {code}";

            if (_environment.IsDevelopment())
            {
                return true;
            }

            return await _smsSender.SendSmsAsync(new SmsLoggingInfo
            {
                PhoneNumber = phoneNumber,
                Message = message,
                NotificationArea = NotificationArea.Registration,
                UserId = userId
            });
        }

        private void LogInformation(string message)
        {
            _logger.Information(message);
        }

        private void LogError(string message)
        {
            _logger.Error(message);
        }

        private bool IsTestUser(ApplicationUser user)
        {
            return _testUserInfoProvider.IsTestUser(user);
        }

        private async Task<ApplicationUser> FindUserByIdAsync(int userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
        #endregion
    }
}