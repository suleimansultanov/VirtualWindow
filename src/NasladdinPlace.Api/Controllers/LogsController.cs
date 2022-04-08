using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.Log;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.Dtos.Pos;
using NasladdinPlace.Logging.Storage;
using Serilog;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.Admin)]
    public class LogsController : Controller
    {
        private readonly ILogsStorage _logsStorage;
        private readonly IPosInteractor _posInteractor;
        private readonly ILogger _logger;

        public LogsController(ILogsStorage logsStorage,
                              IPosInteractor posInteractor,
                              ILogger logger)
        {
            _logsStorage = logsStorage;
            _posInteractor = posInteractor;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var logs = _logsStorage.GetAll().OrderByDescending(l => l.Timestamp);
            var dtos = logs.Select(Mapper.Map<LogDto>).ToImmutableList();

            var orderNumber = 1;
            dtos.ForEach(dto =>
            {
                dto.OrderNumber = orderNumber++;
            });

            return Ok(dtos);
        }

        [HttpPost]
        public IActionResult LoadArchiveLogs([FromBody] PosLogTypeDto logTypeDto)
        {
            if (!Enum.IsDefined(typeof(PosLogType), logTypeDto.PosLogType))
                return BadRequest("Невозможно определить тип запрашиваемых логов");

            var logType = (PosLogType)logTypeDto.PosLogType;
            var posId = logTypeDto.PosId;

            //TODO: Переделать работу с логами витрин вне БД. 
            //await _posInteractor.RequestLogsAsync(posId, logType);

            _logger.Information($"Requested {logType} logs for posId = {posId}.");
            return Ok();
        }
    }
}
