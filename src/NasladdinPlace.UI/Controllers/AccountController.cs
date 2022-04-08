using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Contracts;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Models;
using NasladdinPlace.Api.Client.Rest.Dtos.Account;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _identityInternalSignInManager;
        private readonly INasladdinApiClient _nasladdinApiClient;
        private readonly IAuthTokenManager _externalAuthTokenManagerService;
        private readonly RoleManager<Role> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> identityInternalSignInManager,
            INasladdinApiClient nasladdinApiClient,
            IAuthTokenManager externalAuthTokenManagerService,
            RoleManager<Role> roleManager)
        {
            if(userManager == null)
                throw new ArgumentNullException(nameof(userManager));
            if (identityInternalSignInManager == null)
                throw new ArgumentNullException(nameof(identityInternalSignInManager));
            if (nasladdinApiClient == null)
                throw new ArgumentNullException(nameof(nasladdinApiClient));
            if (externalAuthTokenManagerService == null)
                throw new ArgumentNullException(nameof(externalAuthTokenManagerService));
            if (roleManager == null)
                throw new ArgumentNullException(nameof(roleManager));

            _userManager = userManager;
            _identityInternalSignInManager = identityInternalSignInManager;
            _externalAuthTokenManagerService = externalAuthTokenManagerService;
            _nasladdinApiClient = nasladdinApiClient;
            _roleManager = roleManager;
        }

        public IActionResult Login(string returnUrl = "/")
        {
            return View(new LoginViewModel { RedirectUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);

            var user = await _userManager.FindByNameAsync(loginViewModel.UserName);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Пользователь не существует");
                return View(loginViewModel);
            }

            if (!user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "У вас нет доступа в админку");
                return View(loginViewModel);
            }

            var result = await _identityInternalSignInManager.PasswordSignInAsync(
                loginViewModel.UserName,
                loginViewModel.Password,
                isPersistent: true,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                var dto = new LoginDto
                {
                    UserName = loginViewModel.UserName,
                    Password = loginViewModel.Password
                };

                var userLoggingInResponse = await _nasladdinApiClient.LoginUserAsync(dto);

                if (userLoggingInResponse.Status == ResultStatus.Success)
                {
                    var authPayload = userLoggingInResponse.Result;
                    var tokenExpirationInterval = TimeSpan.FromSeconds(authPayload.ExpiresIn);
                    var authToken = new AuthToken(authPayload.Token, tokenExpirationInterval);
                    await _externalAuthTokenManagerService.UpdateAsync(authToken);
                    await AddRoleClaimsAsync(user);
                    return RedirectToAction("All", "PointsOfSale");
                }

                ModelState.AddModelError(string.Empty, "Авторизация через API не удалась");
                return View(loginViewModel);
            }

            if (result.IsLockedOut)
            {
                return this.RedirectToHome();
            }

            ModelState.AddModelError(string.Empty, "Неверное имя пользователя или пароль");
            return View(loginViewModel);
        }

        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await _externalAuthTokenManagerService.RemoveAuthTokenAsync();

            return this.RedirectToHome();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private async Task AddRoleClaimsAsync(ApplicationUser user)
        {
            if (User == null) return;

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>();

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var claimsIdentity= new ClaimsIdentity(claims);
            User.AddIdentity(claimsIdentity);
        }
    }
}