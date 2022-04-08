using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Payment.Models;
using NasladdinPlace.Core.Services.Payment.Printer.Contracts;
using NasladdinPlace.Core.Services.Payment.Printer.Models;
using NasladdinPlace.Core.Services.Shared.Models;
using NasladdinPlace.Payment.Models;
using NasladdinPlace.Payment.Services;
using System;
using System.Threading.Tasks;
using Currency = NasladdinPlace.Payment.Models.Currency;

namespace NasladdinPlace.Core.Services.Check.Simple.Payment
{
    public class CheckPaymentService : ICheckPaymentService
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentDescriptionPrinter _paymentDescriptionPrinter;

        public CheckPaymentService(
            IPaymentService paymentService,
            IPaymentDescriptionPrinter paymentDescriptionPrinter)
        {
            if (paymentService == null)
                throw new ArgumentNullException(nameof(paymentService));
            if (paymentDescriptionPrinter == null)
                throw new ArgumentNullException(nameof(paymentDescriptionPrinter));

            _paymentService = paymentService;
            _paymentDescriptionPrinter = paymentDescriptionPrinter;
        }

        public async Task<CheckPaymentResult> PayForCheckAsync(IUnitOfWork unitOfWork, int userId, PaymentInfo paymentInfo)
        {
            if (paymentInfo.IsFreeOrEmpty)
                return CheckPaymentResult.NoPaymentRequired();

            var checkPaymentInfo = new CheckPaymentInfo(
                checkCost: paymentInfo.CostWithDiscount,
                bonusAmount: paymentInfo.WrittenOffBonusAmount
            );

            int? transactionId = null;
            int? paymentCardId = null;
            if (checkPaymentInfo.ShouldPayViaMoney)
            {
                var paymentCard = paymentInfo.PaymentCard;
                if (paymentCard == null)
                    return CheckPaymentResult.Failure(
                        Error.FromDescription("Cannot perform recurrent payment. Banking card token does not exist.")
                    );

                paymentCardId = paymentCard.Id;

                var paymentAmount = checkPaymentInfo.CheckCostInMoney;
                var cardToken = paymentCard.Token;

                var activeOperationOfUser =
                    await unitOfWork.PosOperations.GetAsync(paymentInfo.PosOperationId);

                var paymentResponse = await PerformPaymentAsync(paymentAmount, cardToken, userId,
                    _paymentDescriptionPrinter.Print(new PaymentDetails
                    {
                        PointOfSale = activeOperationOfUser.Pos
                    }));

                if (!paymentResponse.IsSuccess || paymentResponse.Result.PaymentStatus != PaymentStatus.Paid)
                {
                    Error errorFromDescriptionWithLocalization;
                    string errorDescription;
                    var localizedDescription = paymentResponse.Result?.LocalizedError ?? string.Empty;
                    if (!paymentResponse.IsSuccess)
                    {
                        errorDescription =
                            $"User {userId} has tried to pay for the purchase but the payment operation has failed.";
                        errorFromDescriptionWithLocalization = Error.FromDescriptionWithLocalization(
                            errorDescription,
                            localizedDescription
                        );
                        return CheckPaymentResult.Failure(errorFromDescriptionWithLocalization);
                    }

                    var bankTransactionError = paymentResponse.Result.LocalizedError ?? paymentResponse.Error;

                    errorDescription =
                        $"User {userId} has tried to pay for the purchase but the payment operation has failed.";
                    errorFromDescriptionWithLocalization =
                        Error.FromDescriptionWithLocalization(errorDescription, localizedDescription);
                    return CheckPaymentResult.FailureWithBankRequisites(
                        errorFromDescriptionWithLocalization,
                        paymentResponse.Result.TransactionId,
                        paymentCardId.Value,
                        checkPaymentInfo,
                        bankTransactionError);
                }

                transactionId = paymentResponse.Result.TransactionId;
            }

            await unitOfWork.CompleteAsync();

            if (transactionId.HasValue)
            {
                return CheckPaymentResult.Paid(
                    transactionId: transactionId.Value,
                    paymentCardId: paymentCardId.Value,
                    checkPaymentInfo: checkPaymentInfo
                );
            }

            return CheckPaymentResult.PaidViaBonuses(checkPaymentInfo);
        }

        private Task<Response<PaymentResult>> PerformPaymentAsync(
            decimal moneyAmount, string cardToken, int userId, string description)
        {
            var recurrentPaymentRequest = new RecurrentPaymentRequest(
                amount: moneyAmount,
                currency: Currency.Rubles,
                cardToken: cardToken,
                userIdentifier: userId.ToString()
            )
            { Description = description };

            return _paymentService.MakeRecurrentPaymentAsync(recurrentPaymentRequest);
        }
    }
}