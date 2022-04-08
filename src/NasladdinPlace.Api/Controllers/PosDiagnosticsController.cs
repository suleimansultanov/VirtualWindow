using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Dtos.PosDiagnostics;
using NasladdinPlace.Api.Extensions;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.PosDiagnostics.Contracts;
using NasladdinPlace.Core.Services.PosDiagnostics.Manager;
using NasladdinPlace.Core.Services.PosDiagnostics.Models;
using NasladdinPlace.DAL.Constants;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class PosDiagnosticsController : Controller
    {
        private readonly IPosDiagnosticsFactory _posDiagnosticsFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPosDiagnosticsManager _posDiagnosticsManager;

        public PosDiagnosticsController(
            IPosDiagnosticsFactory posDiagnosticsFactory,
            UserManager<ApplicationUser> userManager,
            IPosDiagnosticsManager posDiagnosticsManager)
        {
            _posDiagnosticsFactory = posDiagnosticsFactory;
            _userManager = userManager;
            _posDiagnosticsManager = posDiagnosticsManager;
        }

        [HttpPost("api/pointsOfSale/{posId}/diagnostics")]
        public IActionResult RunDiagnosticsForPos(int posId, [FromBody] PosDiagnosticsSettingsDto posDiagnosticsSettingsDto)
        {
            var userId = _userManager.GetUserIdAsInt(User);
            var posDiagnosticsContext = new PosDiagnosticsContext(userId, posId);
            var posDiagnostics = _posDiagnosticsFactory.Create(
                posDiagnosticsSettingsDto.PosDiagnosticsType.Value,
                posDiagnosticsContext
            );

            Task.Run(() => posDiagnostics.PerformAsync());

            return Ok();
        }

        [HttpPost("api/pointsOfSale/diagnostics")]
        public IActionResult RunDiagnostics()
        {
            Task.Run(() => _posDiagnosticsManager.RunPosDiagnosticsAsync());

            return Ok();
        }
    }
}