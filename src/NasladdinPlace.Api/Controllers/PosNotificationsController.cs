using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Core.Services.PosNotifications.Manager.Contracts;
using NasladdinPlace.DAL.Constants;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.Admin)]
    public class PosNotificationsController : Controller
    {
            private readonly IPosNotificationsManager _posNotificationsManager;

            public PosNotificationsController(IPosNotificationsManager posNotificationsManager)
            {
                _posNotificationsManager = posNotificationsManager;
            }

            [HttpPost]
            public IActionResult RunConditionalPurchasesAgent()
            {
                Task.Run(() =>
                    _posNotificationsManager.FindPosWithDisabledNotificationsAsync());

                return Ok();
            }
        }
    }