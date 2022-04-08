using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Client.Rest.Dtos.Account;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;
using NasladdinPlace.Core.Models;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.TestUtils;
using NasladdinPlace.UI.Controllers;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.Tests.DependencyInjection;
using NasladdinPlace.UI.ViewModels;
using NUnit.Framework;

namespace NasladdinPlace.UI.Tests.Scenarios.Auth
{
    public class AccountControllerShould : TestsBase
    {
        private const string UserName = "testuser@domain.com";
        private const string UserPassword = "Testpassword_312";
        private const bool IsActive = true;
        
        private AccountController _accountController;

        private IServiceProvider _serviceProvider;
        private UserManager<ApplicationUser> _userManager;

        public override void SetUp()
        {   
            base.SetUp();
            
            _serviceProvider = new ServiceProviderFactory().CreateServiceProvider(Context);
            
            var mockNasladdinApiClient = _serviceProvider.GetRequiredService<Mock<INasladdinApiClient>>();
            mockNasladdinApiClient
                .Setup(c => c.LoginUserAsync(It.IsAny<LoginDto>()))
                .Returns(() =>
                {
                    var authPayload = new AuthPayloadDto
                    {
                        Token = "Auth token",
                        ExpiresIn = 1000
                    };
                    var requestResult = RequestResponse<AuthPayloadDto>.Success(authPayload);
                    return Task.FromResult(requestResult);
                });
            
            _accountController = _serviceProvider.GetRequiredService<AccountController>();
            _userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }
        
        [Test]
        public void LoginAdminUserWhenProvidedCorrectUserNameAndPassword()
        {
            CreateAdminUser(UserName, UserPassword).Wait();
            
            var loginViewModel = new LoginViewModel
            {
                UserName = UserName,
                Password = UserPassword
            };

            var result = _accountController.Login(loginViewModel).Result;
            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Test]
        public void ReturnLoginErrorWhenUserIsNotExists()
        {
            var nonExistentEmail = "test@test.com";

            var loginViewModel = new LoginViewModel
            {
                UserName = nonExistentEmail,
                Password = UserPassword
            };

            var result = _accountController.Login(loginViewModel).Result;
            result.Should().BeOfType<ViewResult>();
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
    }
}