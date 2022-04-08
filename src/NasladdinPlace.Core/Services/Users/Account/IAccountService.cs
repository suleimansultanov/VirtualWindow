using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Utilities.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Users.Account
{
    public interface IAccountService
    {
        Task<ValueResult<UnverifiedUserInfo>> RegisterUserAsync(UserRegistrationInfo userRegistrationInfo);
        Task<ValueResult<VerifiedUserInfo>> VerifyPhoneNumberAsync(VerificationPhoneNumberInfo verificationPhoneNumberInfo);
        Task<ValueResult<ApplicationUser>> GetUserAccountAsync(int userId);
        Task<Result> SendVerificationCodeAsync(int userId, string phoneNumber);
        Task<Result> ChangePhoneNumberAsync(int userId, ChangePhoneNumberInfo changePhoneNumberInfo);
        Task<Result> PatchUserAsync(UserInfo userInfo);
        Task<Result> UpdateFirebaseTokenAsync(int userId, string token, Brand brand);
    }
}
