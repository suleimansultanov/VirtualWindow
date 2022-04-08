using System;
using System.Threading.Tasks;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Models;
using NasladdinPlace.Core.Services.Payment.Card;
using NasladdinPlace.Core.Services.Users.Test;
using NasladdinPlace.Payment.Models;
using NasladdinPlace.Payment.Services;
using Currency = NasladdinPlace.Payment.Models.Currency;

namespace NasladdinPlace.Core.Services.BankingCardConfirmation.Contracts
{
    public class PaymentCardConfirmationService : IPaymentCardConfirmationService
    {
        private const string ConfirmationFailedErrorFormat = 
            "User's {0} banking card confirmation has been failed because {1}.";

        private const string UnknownError = "Unknown error";

        private const string NetworkError = "Network error";
        
        private const decimal OneRuble = 1.0M;

        private const string PaymentTestDescription = "test";

        public event EventHandler<BankingCardConfirmationNotificationEventArgs> ConfirmationProgressUpdated;

        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IForm3DsHtmlMaker _form3DsHtmlMaker;
        private readonly ITestUserInfoProvider _testUserInfoProvider;

        public PaymentCardConfirmationService(
            IPaymentService paymentService,
            IUnitOfWorkFactory unitOfWorkFactory,
            IForm3DsHtmlMaker form3DsHtmlMaker,
            ITestUserInfoProvider testUserInfoProvider)
        {
            if (paymentService == null)
                throw new ArgumentNullException(nameof(paymentService));
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (form3DsHtmlMaker == null)
                throw new ArgumentNullException(nameof(form3DsHtmlMaker));
            if (testUserInfoProvider == null)
                throw new ArgumentNullException(nameof(testUserInfoProvider));
            
            _paymentService = paymentService;
            _unitOfWorkFactory = unitOfWorkFactory;
            _form3DsHtmlMaker = form3DsHtmlMaker;
            _testUserInfoProvider = testUserInfoProvider;
        }

        public Task<PaymentCardConfirmationResult> TryConfirmPaymentCardViaPaymentServiceAsync(
            IPaymentService paymentService, 
            int userId,
            PaymentCardConfirmationRequest confirmationRequest)
        {
            if (paymentService == null)
                throw new ArgumentNullException(nameof(paymentService));
            if (confirmationRequest == null)
                throw new ArgumentNullException(nameof(confirmationRequest));
            
            return TryConfirmBankingCardAuxAsync(paymentService, userId, confirmationRequest);
        }

        public Task<PaymentCardConfirmationResult> TryConfirmPaymentCardAsync(
            int userId, PaymentCardConfirmationRequest confirmationRequest)
        {
            return TryConfirmPaymentCardViaPaymentServiceAsync(_paymentService, userId, confirmationRequest);
        }

        public async Task<PaymentCardConfirmationResult> CompletePaymentCardConfirmationAsync(
            int userId, Payment3DsCompletionRequest payment3DsCompletionRequest)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return await CompleteBankingCardConfirmationAux(unitOfWork, userId, payment3DsCompletionRequest);
            }
        }

        private async Task<PaymentCardConfirmationResult> TryConfirmBankingCardAuxAsync(
            IPaymentService paymentService,
            int userId, 
            PaymentCardConfirmationRequest confirmationRequest)
        {
            if (await ShouldReturnSuccessResultAsync(userId))
                return PaymentCardConfirmationResult.ConfirmationSuccessful();
            
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var user = await unitOfWork.Users.GetByIdIncludingPaymentCardsFirebaseTokensAndUserBonusPointsAsync(userId);

                NotifyProgressUpdated(
                    BankingCardConfirmationNotificationEventArgs.WithStatus(
                        user,
                        PaymentCardConfirmationStatus.ConfirmationInitiated
                    )
                );

                var userIpAddress = confirmationRequest.UserIpAddress;
                var paymentRequest = new PaymentRequest(
                    OneRuble,
                    Currency.Rubles,
                    confirmationRequest.CardCryptogramPacket,
                    confirmationRequest.CardHolder,
                    userIpAddress,
                    userId.ToString()
                ) {Description = PaymentTestDescription};

                user.NotifyBankingCardVerificationInitiation();
                await unitOfWork.CompleteAsync();

                var paymentResponse = await paymentService.AuthPaymentAsync(paymentRequest);

                return await ProcessPaymentResponse(unitOfWork, user, paymentResponse,
                    confirmationRequest.CryptogramSource);
            }
        }

        private async Task<PaymentCardConfirmationResult> CompleteBankingCardConfirmationAux(
            IUnitOfWork unitOfWork, int userId, Payment3DsCompletionRequest payment3DsCompletionRequest)
        {
            var user = await unitOfWork.Users.GetByIdIncludingPaymentCardsFirebaseTokensAndUserBonusPointsAsync(userId);

            NotifyProgressUpdated(
                BankingCardConfirmationNotificationEventArgs.WithStatus(
                    user,
                    PaymentCardConfirmationStatus.Authorization3DsCompletionInitiated
                )
            );

            var paymentResponse = 
                await _paymentService.Complete3DsPaymentAsync(payment3DsCompletionRequest);

            return await ProcessPaymentResponse(unitOfWork, user, paymentResponse, PaymentCardCryptogramSource.Common);
        }

        private async Task<PaymentCardConfirmationResult> ProcessPaymentResponse(
            IUnitOfWork unitOfWork, 
            ApplicationUser user, 
            Response<PaymentResult> paymentResponse,
            PaymentCardCryptogramSource cryptogramSource)
        {
            string error;
            var userId = user.Id; 
            
            if (!paymentResponse.IsSuccess)
            {
                error = CreateFormattedError(userId, NetworkError);
                NotifyProgressUpdated(
                    BankingCardConfirmationNotificationEventArgs.ConfirmationFailed(
                        user, error, string.Empty
                    )
                );
                return PaymentCardConfirmationResult.ConfirmationFailed(error);
            }

            var paymentResult = paymentResponse.Result;
            switch (paymentResult.PaymentStatus)
            {
                case PaymentStatus.NotPaid:
                    error = CreateFormattedError(userId, paymentResult.Error);
                    NotifyProgressUpdated(
                        BankingCardConfirmationNotificationEventArgs.ConfirmationFailed(
                            user, error, paymentResult.LocalizedError
                        )
                    );
                    return PaymentCardConfirmationResult.ConfirmationFailed(paymentResult.LocalizedError);
                case PaymentStatus.Require3Ds:
                    NotifyProgressUpdated(
                        BankingCardConfirmationNotificationEventArgs.WithStatus(
                            user,
                            PaymentCardConfirmationStatus.Require3DsAuthorization
                        )
                    );
                    var form3DsInfo = new Form3DsInfo(userId, paymentResponse.Result.Info3Ds);
                    var form3DsHtmlString = _form3DsHtmlMaker.Make(form3DsInfo);
                    var info3Ds = paymentResponse.Result.Info3Ds;
                    return PaymentCardConfirmationResult.Require3DsAuthorization(form3DsHtmlString, info3Ds);
                case PaymentStatus.Paid:
                    NotifyProgressUpdated(BankingCardConfirmationNotificationEventArgs.ConfirmationSucceeded(user));
                    var extendedPaymentCardInfo = new ExtendedPaymentCardInfo(paymentResponse.Result.PaymentCardInfo)
                    {
                        CryptogramSource = cryptogramSource,
                        PaymentSystem = PaymentSystem.CloudPayments
                    };
                    user.SetActivePaymentCard(extendedPaymentCardInfo);
                    await unitOfWork.CompleteAsync();
                    
                    CancelPayment(paymentResponse.Result);
                    
                    return PaymentCardConfirmationResult.ConfirmationSuccessful();
                default:
                    error = CreateFormattedError(userId, UnknownError);
                    NotifyProgressUpdated(
                        BankingCardConfirmationNotificationEventArgs.ConfirmationFailed(
                            user, error, string.Empty
                        )
                    );
                    return PaymentCardConfirmationResult.ConfirmationFailed(error);
            }
        }

        private void NotifyProgressUpdated(BankingCardConfirmationNotificationEventArgs confirmationNotificationEventArgs)
        {
            try
            {
                ConfirmationProgressUpdated?.Invoke(this, confirmationNotificationEventArgs);
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        private void CancelPayment(PaymentResult paymentResult)
        {
            var paymentCancellationRequest = new PaymentCancellationRequest(paymentResult.TransactionId);
            _paymentService.CancelPaymentAsync(paymentCancellationRequest);
        }

        private async Task<bool> ShouldReturnSuccessResultAsync(int userId)
        {
            var testUserInfoResult = await _testUserInfoProvider.ProvideTestUserInfoAsync(userId);
            return testUserInfoResult.Succeeded && !testUserInfoResult.Value.IsPaymentCardVerificationRequired;
        }
        
        private static string CreateFormattedError(int userId, string error)
        {
            return string.Format(ConfirmationFailedErrorFormat, userId, error);
        }
    }
}