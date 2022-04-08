using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.Account;
using NasladdinPlace.Api.Dtos.User;
using NasladdinPlace.Api.Extensions;
using NasladdinPlace.Api.Services.Brand;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Users.Account;
using NasladdinPlace.Utilities.Models;
using System;
using System.Threading.Tasks;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;

        public AccountController(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException(nameof(serviceProvider));

            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _accountService = serviceProvider.GetRequiredService<IAccountService>();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAccountAsync()
        {
            var userId = _userManager.GetUserIdAsInt(User);

            var userResult = await _accountService.GetUserAccountAsync(userId);

            if (userResult.Succeeded)
                return Ok(Mapper.Map<UserFullInfoDto>(userResult.Value));

            return BadRequest(userResult.Error);
        }

        [HttpPost("user")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUserIfNecessaryAndStartAuthAsync([FromBody] PhoneNumberDto dto)
        {
            var userRegistrationInfo = new UserRegistrationInfo(dto.PhoneNumber);
            var registrationResult = await _accountService.RegisterUserAsync(userRegistrationInfo);

            if (registrationResult.Succeeded)
                return Ok(Mapper.Map<UserShortInfoDto>(registrationResult.Value.User));

            return BadRequest(registrationResult.Error);
        }

        [HttpPatch("user")]
        public async Task<IActionResult> PatchUserAsync([FromBody] UserFormDto userFormDto)
        {
            var userId = _userManager.GetUserIdAsInt(User);
            var userInfo = new UserInfo(userId)
            {
                Email = userFormDto.Email,
                FullName = userFormDto.FullName,
                BirthDateMilliseconds = userFormDto.BirthDateMillis,
                Gender = userFormDto.Gender
            };

            var patchResult = await _accountService.PatchUserAsync(userInfo);

            return CreateActionResultFromResult(patchResult);
        }

        [HttpPost("verifyPhoneNumber")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyPhoneNumberAsync([FromBody] VerifyPhoneNumberDto dto)
        {
            var verificationPhoneNumberInfo = new VerificationPhoneNumberInfo(dto.PhoneNumber, dto.Code);
            var verificationResult = await _accountService.VerifyPhoneNumberAsync(verificationPhoneNumberInfo);

            if (!verificationResult.Succeeded)
                return BadRequest(verificationResult.Error);

            var confirmationResponse = new UserConfirmationResponseDto
            {
                Password = verificationResult.Value.GeneratedPassword,
                UserPreviousRegistrationStatus = verificationResult.Value.PreviousRegistrationStatus
            };

            return Ok(confirmationResponse);
        }

        [HttpPost("changePhoneNumber")]
        public async Task<IActionResult> RequestPhoneNumberVerificationCodeAsync([FromBody] PhoneNumberDto phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber.PhoneNumber)) return BadRequest();

            var userId = _userManager.GetUserIdAsInt(User);

            var sendVerificationCodeResult = await _accountService.SendVerificationCodeAsync(userId, phoneNumber.PhoneNumber);

            return CreateActionResultFromResult(sendVerificationCodeResult);
        }

        [HttpPost("verifyChangedPhoneNumber")]
        public async Task<IActionResult> ChangePhoneNumberAsync([FromBody] VerifyPhoneNumberDto dto)
        {
            var userId = _userManager.GetUserIdAsInt(User);

            var changeNumberInfo = new ChangePhoneNumberInfo(dto.PhoneNumber, dto.Code);

            var result = await _accountService.ChangePhoneNumberAsync(userId, changeNumberInfo);

            return CreateActionResultFromResult(result);
        }

        [HttpPut("firebaseToken")]
        public async Task<IActionResult> UpdateFirebaseTokenAsync([FromBody] PushNotificationsTokenDto pushNotificationsTokenDto)
        {
            var userId = _userManager.GetUserIdAsInt(User);
            var brand = HttpContext.Request.GetBrandHeaderValue();

            var updateFirebaseTokenResult =
                await _accountService.UpdateFirebaseTokenAsync(userId, pushNotificationsTokenDto.Token, brand);

            return CreateActionResultFromResult(updateFirebaseTokenResult);
        }

        private IActionResult CreateActionResultFromResult(Result result)
        {
            if (result.Succeeded)
                return Ok();

            return BadRequest(result.Error);
        }
    }
}
