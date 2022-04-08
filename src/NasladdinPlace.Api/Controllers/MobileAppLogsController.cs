using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.Log;
using NasladdinPlace.Api.Services.Logs.User;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    public class MobileAppLogsController : Controller
    {
        private readonly IUserLogsManager _userLogsManager;

        public MobileAppLogsController(IUserLogsManager userLogsManager)
        {
            _userLogsManager = userLogsManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult PostLogs([FromBody] MobileAppLogsDto mobileAppLogsDto)
        {
            _userLogsManager.SaveLogsAsync(mobileAppLogsDto.UserPhoneNumber, mobileAppLogsDto.Content);

            return Ok();
        }
    }
}