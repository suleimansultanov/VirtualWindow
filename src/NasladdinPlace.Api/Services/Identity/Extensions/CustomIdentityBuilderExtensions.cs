using Microsoft.AspNetCore.Identity;
using NasladdinPlace.Api.Services.Identity.Providers;

namespace NasladdinPlace.Api.Services.Identity.Extensions
{
    public static class CustomIdentityBuilderExtensions
    {
        public static IdentityBuilder AddChangePhoneNumberTotpTokenProvider(this IdentityBuilder builder)
        {
            var userType = builder.UserType;
            var totpProvider = typeof(ChangePhoneNumberTotpTokenProvider<>).MakeGenericType(userType);
            return builder.AddTokenProvider("ChangePhoneNumberTotpTokenProvider", totpProvider);
        }
    }
}