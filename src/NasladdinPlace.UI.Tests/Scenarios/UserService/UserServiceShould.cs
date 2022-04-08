using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Models;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.TestUtils;
using NasladdinPlace.UI.Services.Users;
using NasladdinPlace.UI.Tests.DependencyInjection;
using NasladdinPlace.UI.ViewModels.Users;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Tests.Scenarios.UserService
{
    class UserServiceShould : TestsBase
    {
        private const string UserName = "testuser@domain.com";
        private const string UserNameForEdit = "user@domain.com";
        private const string UserPassword = "Testpassword_312";
        private const string UserPasswordForChange = "Testpassword312_";
        private const int DefaultUserId = 1;
        private const int AdminRoleId = 1;
        private const int LogisticianRoleId = 2;
        private const bool IsActive = true;

        private IUserService _userService;
        private IServiceProvider _serviceProvider;

        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        public override void SetUp()
        {
            base.SetUp();

            _serviceProvider = new ServiceProviderFactory().CreateServiceProvider(Context);
            _userService = _serviceProvider.GetRequiredService<IUserService>();
            _userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _signInManager = _serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
        }

        [Test]
        public void ReturnResultSucceededIfTheUserIsCreated()
        {
            var createUserViewModel = new CreateUserViewModel
            {
                Email = UserName,
                Password = UserPassword,
                PasswordConfirm = UserPassword,
                SelectedRoles = new List<int>(AdminRoleId)
            };

            var result = _userService.CreateUserAsync(createUserViewModel).Result;
            result.Succeeded.Should().BeTrue();
        }

        [Test]
        public void ReturnResultSucceededIfTheUsersPasswordIsChanged()
        {
            var user = CreateAdminUser(UserName, UserPassword).Result;
            var claimsPrincipal = GetClaimsPrincipal(user).Result;

            var changeUserPasswordViewModel = new ChangeUserPasswordViewModel
            {
                Id = DefaultUserId,
                Email = UserName,
                NewPassword = UserPasswordForChange,
                NewPasswordConfirmation = UserPasswordForChange
            };

            var result = _userService.ChangePasswordAsync(changeUserPasswordViewModel, claimsPrincipal).Result;
            result.Succeeded.Should().BeTrue();
        }

        [Test]
        public void ReturnResultSucceededIfTheUserIsEdited()
        {
            var user = CreateAdminUser(UserName, UserPassword).Result;
            var claimsPrincipal = GetClaimsPrincipal(user).Result;

            var userEditViewModel = new UserEditViewModel
            {
                Id = DefaultUserId,
                UserName = UserNameForEdit,
                Email = UserNameForEdit,
                SelectedRoles = new List<int>(AdminRoleId)
            };

            var result = _userService.EditUserAsync(userEditViewModel, claimsPrincipal).Result;
            result.Succeeded.Should().BeTrue();
        }

        [Test]
        public void ReturnResultSucceededIfTheUsersRoleEdited()
        {
            var user = CreateAdminUser(UserName, UserPassword).Result;
            var claimsPrincipal = GetClaimsPrincipal(user).Result;

            var userEditViewModel = new UserEditViewModel
            {
                Id = DefaultUserId,
                UserName = UserName,
                Email = UserName,
                SelectedRoles = new List<int>(LogisticianRoleId)
            };

            var result = _userService.EditUserAsync(userEditViewModel, claimsPrincipal).Result;
            result.Succeeded.Should().BeTrue();
        }

        [Test]
        public void ReturnResultFailedIfUserAlreadyExists()
        {
            CreateUserAsync(UserName, UserPassword).Wait();

            var createUserViewModel = new CreateUserViewModel
            {
                Email = UserName,
                Password = UserPasswordForChange,
                PasswordConfirm = UserPasswordForChange,
                SelectedRoles = new List<int>(AdminRoleId)
            };

            var result = _userService.CreateUserAsync(createUserViewModel).Result;
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public void ReturnResultFailedIfTheUsersPasswordIsNotChanged()
        {
            var user = CreateAdminUser(UserName, UserPassword).Result;
            var claimsPrincipal = GetClaimsPrincipal(user).Result;

            var changeUserPasswordViewModel = new ChangeUserPasswordViewModel
            {
                NewPassword = UserPasswordForChange,
                NewPasswordConfirmation = UserPasswordForChange
            };

            var result = _userService.ChangePasswordAsync(changeUserPasswordViewModel, claimsPrincipal).Result;
            result.Succeeded.Should().BeFalse();
        }

        [Test]
        public void ReturnResultFailedIfTheUserIsNotEdited()
        {
            var user = CreateAdminUser(UserName, UserPassword).Result;
            var claimsPrincipal = GetClaimsPrincipal(user).Result;

            var userEditViewModel = new UserEditViewModel
            {
                UserName = UserNameForEdit,
                Email = UserNameForEdit,
                SelectedRoles = new List<int>(AdminRoleId)
            };

            var result = _userService.EditUserAsync(userEditViewModel, claimsPrincipal).Result;
            result.Succeeded.Should().BeFalse();
        }

        private async Task<ApplicationUser> CreateAdminUser(string userName, string password)
        {
            var user = await CreateUserAsync(userName, password);
            user.SetUserActivity(IsActive);
            await CreateAndAddAdminRoleToUser(user);
            return user;
        }

        private async Task<ApplicationUser> CreateUserAsync(string userName, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName
            };

            var result = await _userManager.CreateAsync(user, password);
            result.Succeeded.Should().BeTrue();

            return user;
        }

        private async Task CreateAndAddAdminRoleToUser(ApplicationUser user)
        {
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<Role>>();

            var roleCreationResult = await roleManager.CreateAsync(Role.FromName(Roles.Admin));
            roleCreationResult.Succeeded.Should().BeTrue();


            var roleAdditionResult = await _userManager.AddToRoleAsync(user, Roles.Admin);
            roleAdditionResult.Succeeded.Should().BeTrue();
        }

        private async Task<ClaimsPrincipal> GetClaimsPrincipal(ApplicationUser user)
        {
            var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            return claimsPrincipal;
        }
    }
}
