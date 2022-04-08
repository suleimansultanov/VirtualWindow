using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.Errors;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Feedback;
using NasladdinPlace.Core.Services.MobileAppsErrors.Models;
using NasladdinPlace.Core.Services.NotificationsCenter;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    public class MobileAppsErrorsController : Controller
    {
        private readonly INotificationsCenter _notificationsCenter;
        private readonly UserManager<ApplicationUser> _userManager;

        public MobileAppsErrorsController(
            INotificationsCenter notificationsCenter,
            UserManager<ApplicationUser> userManager)
        {
            _notificationsCenter = notificationsCenter;
            _userManager = userManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessError([FromBody] MobileAppErrorDto mobileAppErrorDto)
        {
            UserShortInfo userInfo;
            
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                userInfo = new UserShortInfo(user.Id, user.UserName);
            }
            else if (!string.IsNullOrEmpty(mobileAppErrorDto.PhoneNumber))
            {
                userInfo = new UserShortInfo(0, mobileAppErrorDto.PhoneNumber);
            }
            else
            {
                userInfo = new UserShortInfo(0, "Нет информации");
            }
            
            var deviceInformation = new DeviceInfo(
                mobileAppErrorDto.DeviceInfo.DeviceName,
                mobileAppErrorDto.DeviceInfo.OperatingSystem
            );
            var appInfo = new AppInfo(mobileAppErrorDto.AppInfo.AppVersion);

            var mobileAppErrorInfo = new MobileAppError(
                mobileAppErrorDto.Error,
                userInfo,
                appInfo,
                deviceInformation
            )
            {
                ErrorSource = mobileAppErrorDto.ErrorSource
            };

            _notificationsCenter.PostNotification(
                $"{Emoji.No_Mobile_Phones} Произошла ошибка на мобильном устройстве.",
                mobileAppErrorInfo
            );

            return Ok();
        }
    }
}