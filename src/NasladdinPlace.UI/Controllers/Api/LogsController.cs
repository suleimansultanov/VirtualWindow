using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Dtos.Pos;
using NasladdinPlace.UI.Constants;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Services.NasladdinApi;
using NasladdinPlace.UI.ViewModels.Logs;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.Dtos;
using NasladdinPlace.UI.Helpers.ACL;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Route(Routes.Api)]
    [Authorize]
    [Permission(nameof(ReadOnlyAccess))]
    public class LogsController : BaseController
    {
        private readonly INasladdinApiClient _nasladdinApiClient;

        public LogsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _nasladdinApiClient = serviceProvider.GetRequiredService<INasladdinApiClient>();
        }

        [HttpGet("{id}")]
        public IActionResult GetPosLog(int id)
        {
            var posLog = UnitOfWork.PosLogs.GetById(id);

            return posLog == null
                ? NotFound(new ErrorResponseDto {Error = "Информация о записи не найдена"})
                : GetLogFileResult(posLog);
        }

        [HttpPost]
        public async Task<IActionResult> RequestLogsAsync([FromBody] PosLogViewModel posLog)
        {
            var pos = await UnitOfWork.PointsOfSale.GetByIdAsync(posLog.PosId);
            if (pos == null)
                return NotFound(new ErrorResponseDto {Error = "Информация о витрине не найдена"});

            var requestDto = new PosLogTypeDto
            (
                posId: pos.Id,
                posLogType: (int) posLog.LogType
            );

            await _nasladdinApiClient.RequestPosLogsAsync(requestDto);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Permission(nameof(DeleteLogPermission))]
        public async Task<IActionResult> DeletePosLogAsync(int id)
        {
            if (!UnitOfWork.PosLogs.CheckIsAnyById(id))
                return NotFound(new ErrorResponseDto {Error = "Информация о записи не найдена"});

            UnitOfWork.PosLogs.Remove(id);

            await UnitOfWork.CompleteAsync();

            return Ok();
        }

        private IActionResult GetLogFileResult(PosLog logFile)
        {
            var contentType = $"application/zip";
            var fileName = $"{logFile.FileName}.zip";

            return File(logFile.FileContent, contentType, fileName);
        }
    }
}