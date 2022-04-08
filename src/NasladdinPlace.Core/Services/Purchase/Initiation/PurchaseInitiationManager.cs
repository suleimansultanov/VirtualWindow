using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase;
using NasladdinPlace.Core.Services.Pos.Interactor;
using NasladdinPlace.Core.Services.Pos.Interactor.Models;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosByTokenProviders;
using NasladdinPlace.Core.Services.Purchase.Initiation.Contracts;
using NasladdinPlace.Core.Services.Purchase.Initiation.Models;
using NasladdinPlace.Core.Services.Purchase.Initiation.Utilities;
using NasladdinPlace.Core.Services.UserBalanceCalculator;
using NasladdinPlace.Core.Services.Users.Test;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Logging.PurchaseInitiating;
using NasladdinPlace.Logging.PurchaseInitiating.Contracts;

namespace NasladdinPlace.Core.Services.Purchase.Initiation
{
    public class PurchaseInitiationManager : IPurchaseInitiationManager
    {
        private readonly IUserPaymentBalanceCalculator _userPaymentBalanceCalculator;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ITestUserInfoProvider _testUserInfoProvider;
        private readonly IOngoingPurchaseActivityManager _ongoingPurchaseActivityManager;
        private readonly IPosInteractor _posInteractor;
        private readonly IPurchaseInitiationStatusFactory _purchaseInitiationStatusFactory;
        private readonly IPosByTokenProvider _posByTokenProvider;
        private readonly IPurchaseInitiationLogger _purchaseInitiationLogger;

        public PurchaseInitiationManager(IServiceProvider serviceProvider)
        {
            _userPaymentBalanceCalculator = serviceProvider.GetRequiredService<IUserPaymentBalanceCalculator>();
            _unitOfWorkFactory = serviceProvider.GetRequiredService<IUnitOfWorkFactory>();
            _testUserInfoProvider = serviceProvider.GetRequiredService<ITestUserInfoProvider>();
            _ongoingPurchaseActivityManager = serviceProvider.GetRequiredService<IOngoingPurchaseActivityManager>();
            _posInteractor = serviceProvider.GetRequiredService<IPosInteractor>();
            _purchaseInitiationStatusFactory = new PurchaseInitiationStatusFactory();
            _posByTokenProvider = serviceProvider.GetRequiredService<IPosByTokenProvider>();
            _purchaseInitiationLogger = serviceProvider.GetRequiredService<IPurchaseInitiationLogger>();
        }

        public Task<PurchaseInitiationResult> InitiatePurchaseAsync(PurchaseInitiationParams purchaseInitiationParams)
        {
            if (purchaseInitiationParams == null)
                throw new ArgumentNullException(nameof(purchaseInitiationParams));

            return InitiatePurchaseAuxAsync(purchaseInitiationParams);
        }

        private async Task<PurchaseInitiationResult> InitiatePurchaseAuxAsync(PurchaseInitiationParams purchaseInitiationParams)
        {
            var userId = purchaseInitiationParams.UserId;
            var qrCode = purchaseInitiationParams.QrCode;
            var brand = purchaseInitiationParams.Brand;
            var doorPosition = purchaseInitiationParams.DoorPosition;

            _purchaseInitiationLogger.LogStart(PurchaseInitiationPhase.BalanceCalculation, nameof(purchaseInitiationParams), purchaseInitiationParams);
            var paymentBalance = await _userPaymentBalanceCalculator.CalculateForUserAsync(userId);

            _purchaseInitiationLogger.LogFinish(PurchaseInitiationPhase.BalanceCalculation, nameof(paymentBalance), paymentBalance);
            if (!paymentBalance.IsZero)
            {
                return PurchaseInitiationResult.Failure(
                    PurchaseInitiationStatus.NegativeBalance,
                    $"Cannot open the door for user {userId} because one has negative balance."
                );
            }

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                string failureMessage;
                _purchaseInitiationLogger.LogStart(PurchaseInitiationPhase.QrCodeEncryption, nameof(qrCode), qrCode);
                var posResult = await _posByTokenProvider.TryProvideByTokenAsync(qrCode);
                _purchaseInitiationLogger.LogFinish(PurchaseInitiationPhase.QrCodeEncryption, nameof(posResult), posResult);
                if (!posResult.Succeeded)
                {
                    failureMessage = $"The user {userId} is trying to open the left door of the POS " +
                                    $"by providing qr code {qrCode}. However, QR code does not match. Details: {posResult.Error}.";

                    return PurchaseInitiationResult.Failure(
                        PurchaseInitiationStatus.IncorrectQrCode,
                        failureMessage
                    );
                }

                var pos = posResult.Value;

                if (await _testUserInfoProvider.IsTestUserAsync(userId))
                {
                    var testPosOperation = PosOperation.NewOfUserAndPosBuilder(userId, pos.Id)
                        .SetPos(pos)
                        .Build();
                    return PurchaseInitiationResult.Success(testPosOperation);
                }

                if (!pos.IsInServiceOrInTestMode)
                {
                    return PurchaseInitiationResult.Failure(PurchaseInitiationStatus.PurchaseNotAllowed,
                        $"Pos with id {pos.Id} is not in service");
                }

                _purchaseInitiationLogger.LogStart(PurchaseInitiationPhase.TrackingUserActivity, nameof(userId), userId);
                _ongoingPurchaseActivityManager.Users.StartTrackingActivity(userId);
                _purchaseInitiationLogger.LogFinish(PurchaseInitiationPhase.TrackingUserActivity, nameof(userId), userId);

                var operationInitiationParams = PosOperationInitiationParams.ForPurchase(userId, pos.Id, brand, doorPosition);

                _purchaseInitiationLogger.LogStart(PurchaseInitiationPhase.PosServiceRequesting, "user id and pos id", new { userId, pos.Id });
                var posInteractionResult = await _posInteractor.InitiatePosOperationAsync(operationInitiationParams);
                _purchaseInitiationLogger.LogFinish(PurchaseInitiationPhase.PosServiceRequesting, nameof(posInteractionResult), 
                    new
                    {
                        posInteractionResult.Status,
                        PosOperationId = posInteractionResult.PosOperation?.Id
                    }
                );

                if (posInteractionResult.Succeeded)
                {
                    var posOperation = posInteractionResult.PosOperation;
                    posOperation.Pos = pos;

                    var posRealTimeInfo = unitOfWork.PosRealTimeInfos.GetById(pos.Id);
                    if (posRealTimeInfo == null)
                        return PurchaseInitiationResult.Success(posOperation);

                    posRealTimeInfo.IsPurchaseInProgress = true;
                    await unitOfWork.CompleteAsync();
                    _purchaseInitiationLogger.LogFinish(PurchaseInitiationPhase.SuccessPurchaseInitiating, nameof(posRealTimeInfo), posRealTimeInfo);
                    return PurchaseInitiationResult.Success(posOperation);
                }

                failureMessage =
                    $"Unable to initiate pos operation for user {userId} because {posInteractionResult.Error} {posInteractionResult.Status.ToString()}.";

                var purchaseInitiationStatus = _purchaseInitiationStatusFactory.Create(posInteractionResult.Status);

                _purchaseInitiationLogger.LogFinish(PurchaseInitiationPhase.FailedPurchaseInitiating);
                return PurchaseInitiationResult.Failure(purchaseInitiationStatus, failureMessage);
            }
        }
    }
}