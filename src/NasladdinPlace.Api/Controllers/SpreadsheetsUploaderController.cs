using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Spreadsheet.Uploader.Contracts;
using NasladdinPlace.DAL.Constants;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.CommaSeparatedAllRoles)]
    public class SpreadsheetsUploaderController : Controller
    {
        private readonly ISpreadsheetsUploader _spreadsheetsUploader;

        public SpreadsheetsUploaderController(ISpreadsheetsUploader spreadsheetsUploader)
        {
            _spreadsheetsUploader = spreadsheetsUploader;
        }

        [HttpPost("{type}")]
        public IActionResult Upload(int type)
        {
            var success = Enum.IsDefined(typeof(ReportType), type);

            if (!success)
                return BadRequest("The type is not used in the system.");

            Task.Run(() => _spreadsheetsUploader.UploadAsync((ReportType) type));

            return Ok();
        }
    }
}