using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Core;
using NasladdinPlace.DAL.Constants;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.Admin)]
    public class TacticalDiagnosticsController : Controller
    {
        private readonly IDiagnostics _diagnostics;

        public TacticalDiagnosticsController(IDiagnostics diagnostics)
        {
            _diagnostics = diagnostics;
        }

        [HttpPost]
        public IActionResult RunDiagnostics()
        {
            Task.Run(() => _diagnostics.RunAsync());

            return Ok();
        }
    }
}