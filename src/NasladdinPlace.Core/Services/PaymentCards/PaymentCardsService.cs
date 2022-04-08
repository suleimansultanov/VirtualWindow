using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.PaymentCards.Contracts;
using NasladdinPlace.Logging;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.PaymentCards
{
    public class PaymentCardsService : IPaymentCardsService
    {
        private readonly ILogger _logger;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PaymentCardsService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger>();
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
        }

        public async Task<ValueResult<List<PaymentCard>>> DeletePaymentCardAsync(int userId, int paymentCardId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var paymentCard = await unitOfWork.PaymentCardRepository.GetByIdAsync(paymentCardId);

                    if (paymentCard == null)
                        return ValueResult<List<PaymentCard>>.Failure("Payment card not found.");

                    var user = unitOfWork.Users.GetById(userId);

                    if (user.ActivePaymentCardId.HasValue && paymentCard.Id == user.ActivePaymentCardId)
                        return ValueResult<List<PaymentCard>>.Failure(
                            "Sorry, but you can't delete active card. Please make another card active and try delete this card.");

                    paymentCard.MarkAsDeleted();

                    unitOfWork.PaymentCardRepository.Update(paymentCard);
                    await unitOfWork.CompleteAsync();

                    return await ToPaymentCardsValueResult(unitOfWork, user);
                }
                catch (Exception ex)
                {
                    LogError($"Some error has been occured during deleting payment card. Verbose error: {ex}");
                    return ValueResult<List<PaymentCard>>.Failure(
                        "Some error has been occured during deleting payment card.");
                }
            }
        }

        public async Task<ValueResult<PaymentCard>> GetActivePaymentCardAsync(int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user = unitOfWork.Users.GetById(userId);

                    if (!user.ActivePaymentCardId.HasValue)
                        return ValueResult<PaymentCard>.Failure("User has not any active payment card.");

                    var paymentCard = await unitOfWork.PaymentCardRepository.GetByIdAsync(user.ActivePaymentCardId.Value);

                    if (paymentCard == null)
                        return ValueResult<PaymentCard>.Failure("Payment card not found.");

                    return ValueResult<PaymentCard>.Success(paymentCard);
                }
                catch (Exception ex)
                {
                    LogError($"Some error has been occured during getting active payment card. Verbose error: {ex}");
                    return ValueResult<PaymentCard>.Failure(
                        "Some error has been occured during getting active payment card.");
                }
            }
        }

        public async Task<ValueResult<PaymentCard>> GetPaymentCardForPaymentAsync(int userId, int? paymentCardId)
        {
            if (!paymentCardId.HasValue)
                return await GetActivePaymentCardAsync(userId);

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var paymentCard = await unitOfWork.PaymentCardRepository.GetByIdAsync(paymentCardId.Value);

                    if (paymentCard == null)
                        return ValueResult<PaymentCard>.Failure("Payment card not found.");

                    var isPaymentCardBelongsToUser = await IsPaymentCardBelongsToTheUserAsync(userId, paymentCardId);

                    if (!isPaymentCardBelongsToUser.Succeeded)
                        return LogErrorAndReturnFailureResult($"Payment card does not belong to user. UserId = {userId}, PaymentCardId = {paymentCardId}.");

                    if (paymentCard.Status != PaymentCardStatus.AbleToMakePayment)
                        return LogErrorAndReturnFailureResult($"Payment card does not able to make payment. UserId = {userId}, PaymentCardId = {paymentCardId}.");

                    return ValueResult<PaymentCard>.Success(paymentCard);
                }
                catch (Exception ex)
                {
                    LogError($"Some error has been occured during getting active payment card. Verbose error: {ex}");
                    return ValueResult<PaymentCard>.Failure(
                        "Some error has been occured during getting active payment card.");
                }
            }
        }

        public async Task<ValueResult<List<PaymentCard>>> GetAllPaymentCardsAsync(int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var user = unitOfWork.Users.GetById(userId);

                    var gettingPaymentCardsResult = await GetAllPaymentCardsByUser(unitOfWork, user);

                    return gettingPaymentCardsResult.Succeeded
                        ? ValueResult<List<PaymentCard>>.Success(gettingPaymentCardsResult.Value)
                        : ValueResult<List<PaymentCard>>.Success(new List<PaymentCard>());
                }
                catch (Exception ex)
                {
                    LogError($"Some error has been occured during getting all payment cards. Verbose error: {ex}");
                    return ValueResult<List<PaymentCard>>.Failure(
                        "Some error has been occured during getting all payment cards.");
                }
            }
        }

        public async Task<ValueResult<List<PaymentCard>>> SetActivePaymentCardAsync(int userId, int paymentCardId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    var paymentCard = await unitOfWork.PaymentCardRepository.GetByIdAsync(paymentCardId);

                    if (paymentCard == null)
                        return ValueResult<List<PaymentCard>>.Failure("Payment card not found.");

                    if (!paymentCard.ExpirationDate.HasValue)
                        return ValueResult<List<PaymentCard>>.Failure("Payment card does not contain an expiration date.");

                    var user = await unitOfWork.Users.FindByIdIncludePaymentCardsAsync(userId);

                    user.SetActivePaymentCard(paymentCard.Id);
                    unitOfWork.Users.Update(user);
                    await unitOfWork.CompleteAsync();

                    return await ToPaymentCardsValueResult(unitOfWork, user);
                }
                catch (Exception ex)
                {
                    LogError($"Some error has been occured during setting active payment card. Verbose error: {ex}");
                    return ValueResult<List<PaymentCard>>.Failure(
                        "Some error has been occured during setting active payment card.");
                }
            }
        }

        public async Task<Result> IsPaymentCardBelongsToTheUserAsync(int userId, int? paymentCardId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                try
                {
                    if (!paymentCardId.HasValue)
                        return Result.Success();

                    var paymentCard = await unitOfWork.PaymentCardRepository.GetByIdAsync(paymentCardId.Value);

                    if (paymentCard == null)
                        return Result.Failure("Payment card not found.");

                    var user = await unitOfWork.Users.FindByIdIncludePaymentCardsAsync(userId);

                    return user.PaymentCards.Contains(paymentCard)
                        ? Result.Success()
                        : Result.Failure("Payment card not found.");
                }
                catch (Exception ex)
                {
                    LogError($"Some error has been occured during setting active payment card. Verbose error: {ex}");
                    return Result.Failure(
                        "Some error has been occured during checking payment card.");
                }
            }
        }

        private ValueResult<PaymentCard> LogErrorAndReturnFailureResult(string errorMessage)
        {
            LogError(errorMessage);
            return ValueResult<PaymentCard>.Failure(errorMessage);
        }

        private async Task<ValueResult<List<PaymentCard>>> ToPaymentCardsValueResult(IUnitOfWork unitOfWork, ApplicationUser user)
        {
            var gettingPaymentCardsResult = await GetAllPaymentCardsByUser(unitOfWork, user);

            return gettingPaymentCardsResult.Succeeded
                ? ValueResult<List<PaymentCard>>.Success(gettingPaymentCardsResult.Value)
                : ValueResult<List<PaymentCard>>.Failure(gettingPaymentCardsResult.Error);
        }

        private async Task<ValueResult<List<PaymentCard>>> GetAllPaymentCardsByUser(IUnitOfWork unitOfWork,
            ApplicationUser user)
        {
            var paymentCards = await unitOfWork.PaymentCardRepository.GetAllByUserIdAsync(user.Id);

            if (!paymentCards.Any())
                return ValueResult<List<PaymentCard>>.Failure("Payment cards not found.");

            return ValueResult<List<PaymentCard>>.Success(paymentCards);
        }

        private void LogError(string message)
        {
            _logger.LogError(message);
        }
    }
}
