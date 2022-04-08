using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.DAL.Constants;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Purchase.Conditional.Manager.Contracts;
using NasladdinPlace.Core.Utils.ActionExecution.SafeExecutor;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [Authorize(Roles = Roles.Admin)]
    public class ConditionalPurchasesController : Controller
    {
        private readonly IConditionalPurchaseManager _conditionalPurchaseManager;
        private readonly ISafeAsyncActionExecutorWrapperWithExceptionLogging _safeAsyncActionExecutor;

        public ConditionalPurchasesController(
            IConditionalPurchaseManager conditionalPurchaseManager,
            ISafeAsyncActionExecutorWrapperWithExceptionLogging safeAsyncActionExecutor)
        {
            _conditionalPurchaseManager = conditionalPurchaseManager;
            _safeAsyncActionExecutor = safeAsyncActionExecutor;
        }

        [HttpPost]
        public IActionResult RunConditionalPurchasesAgent()
        {
            _safeAsyncActionExecutor.ExecuteAsync(ExecuteConditionalPurchasesHandling);

            return Ok();
        }

        private void ExecuteConditionalPurchasesHandling()
        {
            _conditionalPurchaseManager.MarkPurchasedCheckItemsAsUnverifiedIfAppearedAfterPurchaseAsync().Wait();
            _conditionalPurchaseManager.DeleteUnverifiedCheckItemsInConditionalPurchasesAsync().Wait();
        }
    }
}
