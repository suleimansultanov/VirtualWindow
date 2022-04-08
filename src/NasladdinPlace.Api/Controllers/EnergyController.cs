using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.Energy;
using Serilog;

namespace NasladdinPlace.Api.Controllers
{
    /// <summary>
    /// This controller is used only for the purpose of technical examination of Android/iOS candidates.
    /// </summary>
    [Route(Routes.Api)]
    [AllowAnonymous]
    public class EnergyController : Controller
    {
        private readonly ILogger _logger;

        public EnergyController(ILogger logger)
        {
            _logger = logger;
        }
        
        [HttpPut("operationStatus")]
        public IActionResult UpdateUserLastEnergyOperationStatus([FromBody] EnergyOperationStatusDto energyOperationStatusDto)
        {
            if (Enum.IsDefined(typeof(EnergyOperationStatus), energyOperationStatusDto.Status.Value))
            {
                _logger.Information("Energy operation status has been successfully updated: {@Status}.", energyOperationStatusDto);
                return Ok(energyOperationStatusDto);
            }

            _logger.Error("Energy operation status is incorrect: {@Status}", energyOperationStatusDto);
            return BadRequest("Energy operation status is incorrect.");
        }
    }
}