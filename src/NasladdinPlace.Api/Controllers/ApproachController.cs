using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Api.Dtos.Approach;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Api.Services.Spreadsheet.Providers.Models;
using NasladdinPlace.Utilities.DateTimeConverter;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [AllowAnonymous]
    public class ApproachController : Controller
    {
        private readonly IApproachesHolderProvider _approachesHolderProvider;

        public ApproachController(IApproachesHolderProvider approachesHolderProvider)
        {
            _approachesHolderProvider = approachesHolderProvider;
        }

        [HttpPost]
        public async Task<IActionResult> PostApproachingQuantity([FromBody] ApproachInfoDto approachInfoDto)
        {
            try
            {
                var eventRecords = approachInfoDto.Events.Select(approachEvent =>
                    {
                        var record = Mapper.Map<ApproachRecord>(approachInfoDto);
                        record.ReceivedDate = SharedDateTimeConverter.ConvertDateTimeToString(DateTime.UtcNow);
                        record.TimeInterval =
                            (TimeSpan.FromMilliseconds(approachEvent.TimeEndInMilliseconds) -
                             TimeSpan.FromMilliseconds(approachEvent.TimeBeginningInMilliseconds)).ToString();
                        record.Distances = string.Join(", ", approachEvent.Distances);
                        return record;
                    })
                    .ToList();

                await _approachesHolderProvider.UploadOrCacheRecords(eventRecords);

                return Ok();
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e, null);
                return BadRequest(ModelState);
            }
        }
    }
}