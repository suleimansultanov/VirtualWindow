using System.Threading.Tasks;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.Users.Manager
{
    public interface IUserManager
    {
        Task<UserManagerResult> CreateAsync(ApplicationUser user);
        Task<UserManagerResult> UpdateAsync(ApplicationUser user);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<string> GenerateChangePhoneNumberTokenAsync(ApplicationUser user, string phoneNumber);
        Task<UserManagerResult> ChangePhoneNumberAsync(ApplicationUser user, string phoneNumber, string changeToken);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<UserManagerResult> ResetPasswordAsync(ApplicationUser user, string resetToken, string password);
        Task<ApplicationUser> FindByIdAsync(int userId);
    }
}