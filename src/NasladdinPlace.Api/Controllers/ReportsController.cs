using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.Core.Utils.ActionExecution.SafeExecutor;
using NasladdinPlace.DAL.Constants;
using NasladdinPlace.Reports.DailyReports.Factory;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.Admin)]
    public class ReportsController : Controller
    {
        private readonly IDailyReportsRunnerFactory _dailyReportsRunnerFactory;
        private readonly ISafeAsyncActionExecutorWrapperWithExceptionLogging _safeAsyncActionExecutor;

        public ReportsController(
            IDailyReportsRunnerFactory dailyReportsRunnerFactory,
            ISafeAsyncActionExecutorWrapperWithExceptionLogging safeAsyncActionExecutor)
        {
            _dailyReportsRunnerFactory = dailyReportsRunnerFactory;
            _safeAsyncActionExecutor = safeAsyncActionExecutor;
        }

        [HttpPost]
        public IActionResult RunDailyStatisticsReport()
        {
            _safeAsyncActionExecutor.ExecuteAsync(async () =>
            {
                var reportsRunner = _dailyReportsRunnerFactory.Create();
                await reportsRunner.RunAsync();
            });

            return Ok();
        }
    }
}