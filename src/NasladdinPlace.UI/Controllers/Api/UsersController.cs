using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Dtos.Shared;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Services.Users;
using NasladdinPlace.UI.ViewModels.Users;
using System;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Dtos;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Route(Routes.Api)]
    [Authorize]
    [Permission(nameof(UserManagementPermission))]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _userService = serviceProvider.GetRequiredService<IUserService>();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorsResponseDto
                {
                    Errors = ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                });

            var createResult = await _userService.CreateUserAsync(viewModel);

            if (!createResult.Succeeded)
                return BadRequest(new ErrorResponseDto() { Error = createResult.Error });

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> EditUserAsync([FromBody] UserEditViewModel userEditViewModel)
        {
            var editUserResult = await _userService.EditUserAsync(userEditViewModel, User);

            if (!editUserResult.Succeeded)
                return BadRequest(new ErrorResponseDto { Error = editUserResult.Error });

            return Ok();
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangeUserPasswordViewModel changeUserPasswordViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorsResponseDto
                {
                    Errors = ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage)
                });

            var changePasswordResult = await _userService.ChangePasswordAsync(changeUserPasswordViewModel, User);

            if (!changePasswordResult.Succeeded)
                return BadRequest(new ErrorsResponseDto { Errors = changePasswordResult.Error.Split('.') });

            return Ok();
        }
    }
}