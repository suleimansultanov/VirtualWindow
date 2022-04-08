using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using Serilog;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [AllowAnonymous]
    public class IvideonController : Controller
    {
        private readonly ILogger _logger;

        public IvideonController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult AcceptAuthCode(string code)
        {
            _logger.Information($"Auth code received from Ivideon is { code }.");

            return string.IsNullOrEmpty(code) 
                ? BadRequest() 
                : (IActionResult) Ok();
        }
    }
}