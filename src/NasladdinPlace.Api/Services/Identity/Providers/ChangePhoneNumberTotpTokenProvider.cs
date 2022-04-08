using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NasladdinPlace.Api.Services.Identity.Security;

namespace NasladdinPlace.Api.Services.Identity.Providers
{
    public class ChangePhoneNumberTotpTokenProvider<TUser> : TotpSecurityStampBasedTokenProvider<TUser> where TUser : class
    {
        public override Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            return GenerateAuxAsync(purpose, manager, user);
        }

        private async Task<string> GenerateAuxAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            var token = await manager.CreateSecurityTokenAsync(user);

            return MutableRfc6238AuthenticationService
                .GenerateCode(token, await GetUserModifierAsync(purpose, manager, user))
                .ToString("D6", CultureInfo.InvariantCulture);
        }

        public override Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            return ValidateAuxAsync(purpose, token, manager, user);
        }

        private async Task<bool> ValidateAuxAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            if (!int.TryParse(token, out var code))
                return false;

            var securityToken = await manager.CreateSecurityTokenAsync(user);
            var userModifierAsync = await GetUserModifierAsync(purpose, manager, user);

            return securityToken != null &&
                   MutableRfc6238AuthenticationService.ValidateCode(securityToken, code, userModifierAsync);
        }

        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            return Task.FromResult(false);
        }

        public override Task<string> GetUserModifierAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            return GetUserModifierAuxAsync(purpose, manager, user);
        }

        private static async Task<string> GetUserModifierAuxAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            return "ChangePhoneNumber:" + purpose + ":" + await manager.GetPhoneNumberAsync(user);
        }
    }
}