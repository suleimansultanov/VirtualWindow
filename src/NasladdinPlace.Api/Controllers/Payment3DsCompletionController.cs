using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Dtos.Payment;
using NasladdinPlace.Api.Dtos.PaymentCard;
using NasladdinPlace.Api.Extensions;
using NasladdinPlace.Api.ViewModels;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Contracts;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Models;
using NasladdinPlace.Core.Services.Configuration.Reader;
using NasladdinPlace.Payment.Models;

namespace NasladdinPlace.Api.Controllers
{
    [Route("api/payment3DsCompletion")]
    public class Payment3DsCompletionController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPaymentCardConfirmationService _paymentCardConfirmationService;
        private readonly IConfigurationReader _configurationReader;

        public Payment3DsCompletionController(
            UserManager<ApplicationUser> userManager,
            IPaymentCardConfirmationService paymentCardConfirmationService,
            IConfigurationReader configurationReader)
        {
            _userManager = userManager;
            _paymentCardConfirmationService = paymentCardConfirmationService;
            _configurationReader = configurationReader;
        }
        
        [HttpPost]
        public async Task<IActionResult> Complete3DsPayment([FromBody] PaymentConfirmationDto paymentConfirmationDto)
        {
            var userId = _userManager.GetUserIdAsInt(User);

            var payment3DsCompletionRequest = new Payment3DsCompletionRequest(
                paymentConfirmationDto.TransactionId.Value, paymentConfirmationDto.PaRes
            );

            var bankingCardConfirmationCompletionResult =
                await _paymentCardConfirmationService.CompletePaymentCardConfirmationAsync(
                    userId, payment3DsCompletionRequest
                );

            return Ok(Mapper.Map<PaymentCardConfirmationResultDto>(bankingCardConfirmationCompletionResult));
        }
        
        [HttpPost("browser/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> Complete3DsPaymentViaBrowser(int userId, PaymentConfirmationViewModel paymentConfirmationDto)
        {   
            var payment3DsCompletionRequest = new Payment3DsCompletionRequest(
                paymentConfirmationDto.MD.Value, 
                paymentConfirmationDto.PaRes
            );
            
            var bankingCardConfirmationResult =
                await _paymentCardConfirmationService.CompletePaymentCardConfirmationAsync(
                    userId, payment3DsCompletionRequest
                );
            
            return bankingCardConfirmationResult.ConfirmationStatus ==
                   PaymentCardConfirmationStatus.ConfirmationSucceeded
                ? RedirectToPayment3DsCompletionSuccessPage()
                : RedirectToPayment3DsCompletionFailurePage(bankingCardConfirmationResult.Error);
        }

        private IActionResult RedirectToPayment3DsCompletionSuccessPage()
        {
            var baseAdminUrl = _configurationReader.GetAdminPageBaseUrl();
            var paymentSuccessUrl = _configurationReader.GetPayment3DsResultUrlsSuccess();

            return Redirect(ConfigurationReaderExt.CombineUrlParts(baseAdminUrl, paymentSuccessUrl));
        }

        private IActionResult RedirectToPayment3DsCompletionFailurePage(string error)
        {
            var baseAdminUrl = _configurationReader.GetAdminPageBaseUrl();
            var paymentFailureUrl = _configurationReader.GetPayment3DsResultUrlsFailure(error);
            return Redirect(ConfigurationReaderExt.CombineUrlParts(baseAdminUrl, paymentFailureUrl));
        }

    }
}