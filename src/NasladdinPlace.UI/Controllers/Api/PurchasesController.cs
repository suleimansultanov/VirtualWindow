using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Authorization.AppFeatures;
using NasladdinPlace.UI.Controllers.Base;
using NasladdinPlace.UI.Extensions;
using NasladdinPlace.UI.Helpers.ACL;
using NasladdinPlace.UI.Services.NasladdinApi;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Controllers.Api
{
    [Authorize]
    [Permission(nameof(DocumentGoodsMovingPermission))]
    public class PurchasesController : BaseController
    {
        private readonly INasladdinApiClient _nasladdinApiClient;

        public PurchasesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _nasladdinApiClient = serviceProvider.GetRequiredService<INasladdinApiClient>();
        }

        [HttpDelete("api/users/{userId}/[controller]/unfinished")]
        public async Task<IActionResult> ResetUserBalanceAsync(int userId)
        {
            var resetUserBalanceResult = await _nasladdinApiClient.ResetUserBalanceAsync(userId);

            return resetUserBalanceResult.ToActionResult();
        }

        [HttpPatch("api/[controller]/{userShopTransactionId}/bankTransactionInfos/{bankTransactionInfoId}/refund")]
        public async Task<IActionResult> RefundPaymentAsync(int userShopTransactionId, int bankTransactionInfoId)
        {
            var refundPaymentResult = await _nasladdinApiClient.RefundPaymentAsync(userShopTransactionId, bankTransactionInfoId);

            return refundPaymentResult.ToActionResult();
        }
    }
}