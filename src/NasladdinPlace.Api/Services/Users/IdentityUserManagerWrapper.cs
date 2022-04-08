using Microsoft.AspNetCore.Identity;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Users.Manager;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.Users
{
    public class IdentityUserManagerWrapper : IUserManager
    {
        public const string ChangePhoneNumberTokenPurpose = "ChangePhoneNumber";

        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityUserManagerWrapper(UserManager<ApplicationUser> userManager)
        {
            if (userManager == null)
                throw new ArgumentNullException(nameof(userManager));

            _userManager = userManager;
        }

        public async Task<UserManagerResult> CreateAsync(ApplicationUser user)
        {
            var userCreationResult = await _userManager.CreateAsync(user);
            return TransformToUserManagerResult(userCreationResult);
        }

        public async Task<UserManagerResult> UpdateAsync(ApplicationUser user)
        {
            var userUpdateResult = await _userManager.UpdateAsync(user);
            return TransformToUserManagerResult(userUpdateResult);
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return _userManager.FindByNameAsync(userName);
        }

        public Task<string> GenerateChangePhoneNumberTokenAsync(ApplicationUser user, string phoneNumber)
        {
            return _userManager.GenerateUserTokenAsync(
                user, "ChangePhoneNumberTotpTokenProvider", GetTokePhoneNumberPurpose(phoneNumber));
        }

        public Task<UserManagerResult> ChangePhoneNumberAsync(ApplicationUser user, string phoneNumber, string changeToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return ChangePhoneNumberAuxAsync(user, phoneNumber, changeToken);
        }

        private async Task<UserManagerResult> ChangePhoneNumberAuxAsync(ApplicationUser user, string phoneNumber, string changeToken)
        {
            var verifyUserTokenResult = await _userManager.VerifyUserTokenAsync(user,
                "ChangePhoneNumberTotpTokenProvider", GetTokePhoneNumberPurpose(phoneNumber), changeToken);

            var phoneNumberChangingResult = IdentityResult.Failed();

            if (verifyUserTokenResult)
            {
                phoneNumberChangingResult = await _userManager.SetPhoneNumberAsync(user, phoneNumber);
            }

            return TransformToUserManagerResult(phoneNumberChangingResult);
        }

        public Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<UserManagerResult> ResetPasswordAsync(ApplicationUser user, string resetToken, string password)
        {
            var passwordResettingResult = await _userManager.ResetPasswordAsync(user, resetToken, password);
            return TransformToUserManagerResult(passwordResettingResult);
        }

        private static UserManagerResult TransformToUserManagerResult(IdentityResult identityResult)
        {
            return identityResult.Succeeded
                ? UserManagerResult.Success()
                : UserManagerResult.Failure(identityResult.Errors.Select(e => e.ToString()));

        }

        private static string GetTokePhoneNumberPurpose(string phoneNumber)
        {
            return $"{ChangePhoneNumberTokenPurpose}:{phoneNumber}";
        }

        public Task<ApplicationUser> FindByIdAsync(int userId)
        {
            return _userManager.FindByIdAsync(userId.ToString());
        }
    }
}