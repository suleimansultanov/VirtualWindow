using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Utilities.Models;
using NasladdinPlace.UI.ViewModels.Users;
using NasladdinPlace.Core.Models;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.Logging;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.UI.Services.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IPasswordValidator<ApplicationUser> _passwordValidator;

        public UserService(IServiceProvider serviceProvider)
        {
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _passwordValidator = serviceProvider.GetRequiredService<IPasswordValidator<ApplicationUser>>();
            _passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher<ApplicationUser>>();
        }

        public async Task<Result> CreateUserAsync(CreateUserViewModel createUserViewModel)
        {
            var user = new ApplicationUser
            {
                Email = createUserViewModel.Email,
                UserName = createUserViewModel.Email
            };

            user.MarkAsCompletedRegistration();
            user.SetUserActivity(createUserViewModel.IsActive);

            var createUserResult = await _userManager.CreateAsync(user, createUserViewModel.Password);

            if (!createUserResult.Succeeded)
            {
                var errors = string.Join('.', createUserResult.Errors.Select(error => error.Description));
                return Result.Failure(errors);
            }

            AddUserToRoles(user, createUserViewModel.SelectedRoles);

            await _userManager.UpdateAsync(user);

            return Result.Success();
        }

        public async Task<Result> EditUserAsync(UserEditViewModel userEditViewModel, 
            ClaimsPrincipal userPrincipal)
        {
            SharedDateTimeConverter.ConvertFromMoscowToUtcDateTime(userEditViewModel.Birthday,
                out var userBirthdayInUtc);

            var user = await GetUserByIdIncludingUserRolesAsync(userEditViewModel.Id);

            if (user == null)
            {
                LogError($"User with Id {userEditViewModel.Id} not found.");

                return Result.Failure("Информация о пользователе не найдена.");
            }

            UpdateFieldsFromViewModel(user, userEditViewModel, userBirthdayInUtc);

            if (userPrincipal.IsInRole(nameof(Roles.Admin)))
            {
                AddUserToRoles(user, userEditViewModel.SelectedRoles);

                LogInformation($"User {userPrincipal.Identity.Name} has changed Role for user {user.Email} to {userEditViewModel.SelectedRoles}");
            }

            await _userManager.UpdateAsync(user);

            LogInformation($"User {userPrincipal.Identity.Name} has edited user {userEditViewModel.Email}");

            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(ChangeUserPasswordViewModel changeUserPasswordViewModel, 
            ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.FindByIdAsync(changeUserPasswordViewModel.Id.ToString());

            if (user == null)
            {
                LogError($"User with Id {changeUserPasswordViewModel.Id} not found.");

                return Result.Failure("Информация о пользователе не найдена.");
            }

            var validateResult =
                await _passwordValidator.ValidateAsync(_userManager, user, changeUserPasswordViewModel.NewPassword);

            if (!validateResult.Succeeded)
            {
                var errors = string.Join('.', validateResult.Errors.Select(error => error.Description));
                return Result.Failure(errors);
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, changeUserPasswordViewModel.NewPassword);

            await _userManager.UpdateAsync(user);

            LogInformation($"User {userPrincipal.Identity.Name} has changed password for user {changeUserPasswordViewModel.Email}");

            return Result.Success();
        }

        private void AddUserToRoles(ApplicationUser user, 
            IEnumerable<int> roles)
        {
            user.UserRoles.Clear();

            foreach (var roleId in roles)
            {
                var userRole = new UserRole { UserId = user.Id, RoleId = roleId };

                user.UserRoles.Add(userRole);
            }
        }

        private void LogError(string message)
        {
            _logger.LogError(message);
        }

        private void LogInformation(string message)
        {
            _logger.LogInfo(message);
        }

        private void UpdateFieldsFromViewModel(ApplicationUser user, 
            UserEditViewModel userEditViewModel, 
            DateTime userBirthdayInUtc)
        {
            user.Email = userEditViewModel.Email;
            user.PhoneNumber = userEditViewModel.PhoneNumber;
            user.Birthdate = userEditViewModel.Birthdate ?? userBirthdayInUtc;
            user.Gender = userEditViewModel.Gender;
            user.UserName = userEditViewModel.UserName;

            if (userEditViewModel.IsActive != null)
                user.SetUserActivity(userEditViewModel.IsActive.Value);
        }

        private Task<ApplicationUser> GetUserByIdIncludingUserRolesAsync(int userId)
        {
            return _userManager.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.Id == userId);
        }
    }
}
