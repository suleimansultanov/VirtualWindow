using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Dtos.PaymentCard;
using NasladdinPlace.Api.Extensions;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.PaymentCards;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Contracts;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.PaymentCards.Contracts;

namespace NasladdinPlace.Api.Controllers
{
    [Route("api/bankingCards")]
    public class PaymentCardsController : Controller
    {
        private const string DefaultIpAddress = "127.0.0.1";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentCardConfirmationService _paymentCardConfirmationService;
        private readonly IPaymentCardsService _paymentCardsService;

        public PaymentCardsController(IServiceProvider serviceProvider)
        {
            _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            _paymentCardConfirmationService = serviceProvider.GetRequiredService<IPaymentCardConfirmationService>();
            _paymentCardsService = serviceProvider.GetRequiredService<IPaymentCardsService>();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPaymentCardsAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            var paymentCardsResult = await _paymentCardsService.GetAllPaymentCardsAsync(user.Id);

            var paymentCardsDto = PaymentCardsToDto(paymentCardsResult.Value, user.ActivePaymentCardId);

            return Ok(paymentCardsDto);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActivePaymentCardAsync()
        {
            var userId = _userManager.GetUserIdAsInt(User);

            var paymentCardResult = await _paymentCardsService.GetActivePaymentCardAsync(userId);

            if (!paymentCardResult.Succeeded)
                return BadRequest(paymentCardResult.Error);

            var paymentCardDto = Mapper.Map<PaymentCardDto>(paymentCardResult.Value);

            paymentCardDto.IsActive = true;

            return Ok(paymentCardDto);
        }

        [HttpPost("active")]
        public async Task<IActionResult> SetActivePaymentCardAsync([FromBody] ActivePaymentCard paymentCard)
        {
            var userId = _userManager.GetUserIdAsInt(User);

            var paymentCardsResult = await _paymentCardsService.SetActivePaymentCardAsync(userId, paymentCard.Id);

            if (!paymentCardsResult.Succeeded)
                return BadRequest(paymentCardsResult.Error);

            var user = await _userManager.GetUserAsync(User);

            var paymentCardsDto = PaymentCardsToDto(paymentCardsResult.Value, user.ActivePaymentCardId);

            return Ok(paymentCardsDto);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPaymentCardAsync(
            [FromBody] PaymentCardConfirmationDto paymentCardConfirmationDto)
        {
            var userId = _userManager.GetUserIdAsInt(User);

            var userIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            if (string.IsNullOrWhiteSpace(userIpAddress))
            {
                userIpAddress = DefaultIpAddress;
            }

            var paymentCardConfirmationInfo = new PaymentCardConfirmationRequest(
                paymentCardConfirmationDto.CardHolder,
                paymentCardConfirmationDto.CardCryptogramPacket,
                userIpAddress,
                paymentCardConfirmationDto.CryptogramSource ?? PaymentCardCryptogramSource.Common
            );

            var paymentCardConfirmationResult = await _paymentCardConfirmationService.TryConfirmPaymentCardAsync(
                userId, paymentCardConfirmationInfo
            );

            return Ok(ToDto(paymentCardConfirmationResult));
        }

        [HttpDelete("{paymentCardId}")]
        public async Task<IActionResult> DeletePaymentCardAsync(int paymentCardId)
        {
            var userId = _userManager.GetUserIdAsInt(User);

            var deletePaymentCardResult = await _paymentCardsService.DeletePaymentCardAsync(userId, paymentCardId);

            if (!deletePaymentCardResult.Succeeded)
                return BadRequest(deletePaymentCardResult.Error);

            var user = await _userManager.GetUserAsync(User);

            var paymentCardsDto = PaymentCardsToDto(deletePaymentCardResult.Value, user.ActivePaymentCardId);

            return Ok(paymentCardsDto);
        }

        private static PaymentCardConfirmationResultDto ToDto(PaymentCardConfirmationResult paymentCardConfirmationResult)
        {
            return Mapper.Map<PaymentCardConfirmationResultDto>(paymentCardConfirmationResult);
        }

        private List<PaymentCardDto> PaymentCardsToDto(List<PaymentCard> paymentCards, int? activePaymentCardId)
        {
            var paymentCardsDto = Mapper.Map<List<PaymentCardDto>>(paymentCards);

            foreach (var paymentCardDto in paymentCardsDto)
                paymentCardDto.IsActive = activePaymentCardId.HasValue && paymentCardDto.Id == activePaymentCardId;

            return paymentCardsDto;
        }
    }
}
