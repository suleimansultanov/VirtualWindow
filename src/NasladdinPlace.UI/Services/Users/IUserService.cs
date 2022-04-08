using System.Security.Claims;
using System.Threading.Tasks;
using NasladdinPlace.UI.ViewModels.Users;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.UI.Services.Users
{
    public interface IUserService
    {
        Task<Result> CreateUserAsync(CreateUserViewModel createUserViewModel);
        Task<Result> EditUserAsync(UserEditViewModel userEditViewModel, ClaimsPrincipal userPrincipal);
        Task<Result> ChangePasswordAsync(ChangeUserPasswordViewModel changeUserPasswordViewModel, ClaimsPrincipal userPrincipal);
    }
}
