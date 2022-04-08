using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Payment.Printer.Contracts;
using NasladdinPlace.Core.Services.Payment.Printer.Models;
using NasladdinPlace.Payment.Models;
using NasladdinPlace.Payment.Services;
using System.Threading.Tasks;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core.Services.Check.Helpers.Contracts;
using NasladdinPlace.Core.Services.Check.Refund.Models;

namespace NasladdinPlace.Core.Services.Check.Helpers
{
    public class CheckManagerPaymentHelper : ICheckManagerPaymentHelper
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentDescriptionPrinter _paymentDescriptionPrinter;
        private readonly ICheckManagerBonusPointsHelper _bonusPointsHelper;

        public CheckManagerPaymentHelper(
            IPaymentService paymentService,
            IPaymentDescriptionPrinter paymentDescriptionPrinter,
            ICheckManagerBonusPointsHelper bonusPointsHelper)
        {
            _paymentService = paymentService;
            _paymentDescriptionPrinter = paymentDescriptionPrinter;
            _bonusPointsHelper = bonusPointsHelper;
        }

        public async Task<CheckManagerResult> TryPayForCheckItemAsync(
            IUnitOfWork unitOfWork,
            PosOperation posOperation,
            decimal amountViaMoney,
            PosOperationTransactionType transactionType,
            PosOperationTransaction posOperationTransaction = null,
            IReadOnlyCollection<CheckItem> checkItems = null)
        {
            var activePaymentCard = posOperation.User.ActivePaymentCard;

            if (activePaymentCard == null)
                return CheckManagerResult.Failure("Cannot perform recurrent payment. Banking card token does not exist.");

            var amountPayViaBonuses = _bonusPointsHelper.CalculateBonusPointsAmountThatCanBeWrittenOff(posOperation, transactionType, amountViaMoney);
            var amountPayViaMoney = 0M;

            if (amountViaMoney > amountPayViaBonuses)
            {
                amountPayViaMoney = amountViaMoney - amountPayViaBonuses;

                var paymentResponse = await PerformPaymentAsync(
                    amountPayViaMoney,
                    activePaymentCard.Token,
                    posOperation.UserId,
                    _paymentDescriptionPrinter.Print(new PaymentDetails
                    {
                        PointOfSale = posOperation.Pos
                    }));

                if (!paymentResponse.IsSuccess || paymentResponse.Result.PaymentStatus != PaymentStatus.Paid)
                {
                    var errorDescription = string.IsNullOrEmpty(paymentResponse.Result?.LocalizedError)
                        ? paymentResponse.Error ?? string.Empty
                        : paymentResponse.Result?.LocalizedError;

                    var paymentErrorTransactionInfo = BankTransactionInfo.ForError(
                        paymentCardId: activePaymentCard.Id,
                        bankTransactionId: paymentResponse.Result.TransactionId,
                        amount: amountPayViaMoney,
                        comment: errorDescription
                    );
                    posOperation.AddBankTransaction(paymentErrorTransactionInfo);

                    return CheckManagerResult.Failure(paymentResponse.Error);
                }

                var paymentTransactionInfo = BankTransactionInfo.ForPayment(
                    paymentCardId: activePaymentCard.Id,
                    bankTransactionId: paymentResponse.Result.TransactionId,
                    amount: amountPayViaMoney
                );
                posOperation.AddBankTransaction(paymentTransactionInfo);

                if(posOperationTransaction != null)
                {
                    posOperation.AddTransaction(posOperationTransaction);
                    await unitOfWork.CompleteAsync();

                    if (transactionType == PosOperationTransactionType.Verification && checkItems != null)
                        posOperationTransaction.CalculateAndSetAmounts(checkItems, amountPayViaBonuses);
                    
                    var bankTransactionInfoVersionTwo = BankTransactionInfoVersionTwo.ForPayment(
                        paymentCardId: activePaymentCard.Id,
                        bankTransactionId: paymentResponse.Result.TransactionId,
                        amount: amountPayViaMoney
                    );
                    posOperationTransaction.AddBankTransaction(bankTransactionInfoVersionTwo);
                    posOperationTransaction.MarkAsInProcess();
                    posOperationTransaction.MarkAsPaidUnfiscalized();
                }
            }
           
            //TODO make record about subtracted bonusPoints into UsersBonuses (more universal way)

            var checkItemsAmount = checkItems?.Sum(cki => cki.PriceWithDiscount) ?? 0M;

            var bonusPointsToWrittenOff =
                amountPayViaBonuses <= checkItemsAmount || checkItems == null ? amountPayViaBonuses : checkItemsAmount;

            if (bonusPointsToWrittenOff > 0 )
                _bonusPointsHelper.SubtractBonusPointsFromUserAndAddToPosOperation(posOperation, bonusPointsToWrittenOff);

            if (posOperationTransaction != null && amountViaMoney <= amountPayViaBonuses)
            {
                posOperation.AddTransaction(posOperationTransaction);

                if (transactionType == PosOperationTransactionType.Verification && checkItems != null)
                    posOperationTransaction.CalculateAndSetAmounts(checkItems, amountPayViaBonuses);

                posOperationTransaction.MarkAsInProcess();
                posOperationTransaction.MarkAsPaidByBonusPoints();
            }

            var checkEditingInfo = new CheckEditingInfo(posOperation, amountPayViaMoney, CheckEditingType.AdditionOrVerification);

            return CheckManagerResult.Success(checkEditingInfo);
        }

        public Task<Response<OperationResult>> MakeRefundAsync(int bankTransactionId, decimal refundAmount)
        {
            var refundPaymentRequest = new RefundRequest(bankTransactionId, refundAmount);
            return _paymentService.MakeRefundAsync(refundPaymentRequest);
        }

        public Task<Response<PaymentResult>> PerformPaymentAsync(
            decimal moneyAmount, string cardToken, int userId, string description)
        {
            var recurrentPaymentRequest = new RecurrentPaymentRequest(
                moneyAmount,
                NasladdinPlace.Payment.Models.Currency.Rubles,
                cardToken, userId.ToString()
            )
            { Description = description };


            return _paymentService.MakeRecurrentPaymentAsync(recurrentPaymentRequest);
        }
    }
}
