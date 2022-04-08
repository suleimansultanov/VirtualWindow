using CloudPaymentsClient.Domain.Factories.CurrencyCode;
using CloudPaymentsClient.Rest.Api;
using CloudPaymentsClient.Rest.Dtos.Payment;
using CloudPaymentsClient.Rest.Dtos.Payment.Cryptogram;
using CloudPaymentsClient.Rest.Dtos.Payment.Payment3DsCompletion;
using CloudPaymentsClient.Rest.Dtos.Payment.Recurrent;
using CloudPaymentsClient.Rest.Dtos.Payment.Refund;
using CloudPaymentsClient.Rest.Dtos.Shared;
using CloudPaymentsClient.Rest.Mappers;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Payment.Models;
using NasladdinPlace.Payment.Services;
using System;
using System.Threading.Tasks;

namespace CloudPaymentsClient.Domain.Services
{
    internal class CloudPaymentsService : CloudPaymentsBaseService, IPaymentService
    {
        private readonly string _authHeader;
        private readonly ICloudPaymentsApi _cloudPaymentsApi;
        private readonly ICurrencyCodeProvider _currencyCodeProvider;
        private readonly IPaymentResponseToPaymentCardInfoMapper _paymentResponseToPaymentCardInfoMapper;

        public CloudPaymentsService(
            string authHeader,
            ICloudPaymentsApi cloudPaymentsApi,
            ICurrencyCodeProvider currencyCodeProvider,
            IPaymentResponseToPaymentCardInfoMapper paymentResponseToPaymentCardInfoMapper)
        {
            if (string.IsNullOrWhiteSpace(authHeader))
                throw new ArgumentNullException(nameof(authHeader));
            if (cloudPaymentsApi == null)
                throw new ArgumentNullException(nameof(cloudPaymentsApi));
            if (currencyCodeProvider == null)
                throw new ArgumentNullException(nameof(currencyCodeProvider));
            if (paymentResponseToPaymentCardInfoMapper == null)
                throw new ArgumentNullException(nameof(paymentResponseToPaymentCardInfoMapper));

            _authHeader = authHeader;
            _cloudPaymentsApi = cloudPaymentsApi;
            _currencyCodeProvider = currencyCodeProvider;
            _paymentResponseToPaymentCardInfoMapper = paymentResponseToPaymentCardInfoMapper;
        }

        public Task<Response<PaymentResult>> MakePaymentAsync(PaymentRequest paymentRequest)
        {
            return PerformRequest(() => MakePaymentAuxAsync(paymentRequest));
        }

        public Task<Response<PaymentResult>> AuthPaymentAsync(PaymentRequest paymentRequest)
        {
            return PerformRequest(() => AuthPaymentAuxAsync(paymentRequest));
        }

        public Task<Response<PaymentResult>> Complete3DsPaymentAsync(Payment3DsCompletionRequest payment3DsCompletionRequest)
        {
            return PerformRequest(() => Complete3DsPaymentAuxAsync(payment3DsCompletionRequest));
        }

        public Task<Response<PaymentResult>> MakeRecurrentPaymentAsync(RecurrentPaymentRequest recurrentPaymentRequest)
        {
            return PerformRequest(() => MakeRecurrentPaymentAuxAsync(recurrentPaymentRequest));
        }

        public Task<Response<OperationResult>> MakeRefundAsync(RefundRequest refundRequest)
        {
            return PerformRequest(() => MakeRefundAuxAsync(refundRequest));
        }

        public Task<Response<OperationResult>> CancelPaymentAsync(PaymentCancellationRequest paymentCancellationRequest)
        {
            return PerformRequest(() => CancelPaymentAuxAsync(paymentCancellationRequest));
        }

        private async Task<Response<PaymentResult>> MakePaymentAuxAsync(PaymentRequest paymentRequest)
        {
            var paymentRequestDto = TransformPaymentRequestToDto(paymentRequest);
            var result = await _cloudPaymentsApi.MakePaymentAsync(_authHeader, paymentRequestDto);
            return ProcessPaymentResponse(result);
        }

        private async Task<Response<PaymentResult>> AuthPaymentAuxAsync(PaymentRequest paymentRequest)
        {
            var paymentRequestDto = TransformPaymentRequestToDto(paymentRequest);
            var result = await _cloudPaymentsApi.AuthPaymentAsync(_authHeader, paymentRequestDto);
            return ProcessPaymentResponse(result);
        }

        private async Task<Response<PaymentResult>> MakeRecurrentPaymentAuxAsync(
            RecurrentPaymentRequest recurrentPaymentRequest)
        {
            var recurrentPaymentRequestDto = new RecurrentPaymentRequestDto(
                recurrentPaymentRequest.Amount,
                _currencyCodeProvider.Provide(recurrentPaymentRequest.Currency),
                recurrentPaymentRequest.UserIdentifier,
                recurrentPaymentRequest.CardToken
            )
            {
                Description = recurrentPaymentRequest.Description
            };

            var result = await _cloudPaymentsApi.MakeRecurrentPaymentAsync(_authHeader, recurrentPaymentRequestDto);

            return ProcessPaymentResponse(result);
        }

        private Response<PaymentResult> ProcessPaymentResponse(GenericResponseDto<PaymentResponseDto> paymentResponseDto)
        {
            if (paymentResponseDto.Model == null)
                return Response<PaymentResult>.Failure(paymentResponseDto.Message);

            var transactionId = paymentResponseDto.Model.TransactionId;
            switch (paymentResponseDto.Model.ResponseType)
            {
                case PaymentResponseType.PaymentCompleted:
                case PaymentResponseType.PaymentAuthorized:
                    var paymentCardInfo = CreatePaymentCardInfoFromPaymentResponse(paymentResponseDto.Model);
                    return Response<PaymentResult>.Success(
                        PaymentResult.Paid(transactionId, paymentCardInfo)
                    );
                case PaymentResponseType.Required3Ds:
                    var info3Ds = new Info3Ds(
                        paymentResponseDto.Model.PaReq,
                        paymentResponseDto.Model.AcsUrl,
                        paymentResponseDto.Model.TransactionId
                    );
                    return Response<PaymentResult>.Success(PaymentResult.Require3Ds(transactionId, info3Ds));
                case PaymentResponseType.Error:
                    return Response<PaymentResult>.Success(
                        PaymentResult.NotPaid(
                            transactionId,
                            paymentResponseDto.Model.Reason,
                            paymentResponseDto.Model.CardHolderMessage
                        )
                    );
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(paymentResponseDto),
                        paymentResponseDto.Model.ResponseType,
                        $"Unable to find the specified {nameof(PaymentResponseType)}."
                    );
            }
        }

        private async Task<Response<OperationResult>> MakeRefundAuxAsync(RefundRequest refundRequest)
        {
            var refundRequestDto = new RefundRequestDto(refundRequest.TransactionId, refundRequest.Amount);
            var result = await _cloudPaymentsApi.MakeRefundAsync(_authHeader, refundRequestDto);

            return Response<OperationResult>.Success(
                result.Success
                    ? OperationResult.Success()
                    : OperationResult.Failure(result.Message)
            );
        }

        private async Task<Response<PaymentResult>> Complete3DsPaymentAuxAsync(
            Payment3DsCompletionRequest payment3DsCompletionRequest)
        {
            var payment3DsCompletionRequestDto = new Payment3DsCompletionRequestDto(
                payment3DsCompletionRequest.TransactionId, payment3DsCompletionRequest.PaRes
            );

            var paymentResponse = await _cloudPaymentsApi.Complete3DsPaymentAsync(
                _authHeader, payment3DsCompletionRequestDto
            );

            return ProcessPaymentResponse(paymentResponse);
        }

        private async Task<Response<OperationResult>> CancelPaymentAuxAsync(
            PaymentCancellationRequest paymentCancellationRequest)
        {
            var paymentCancellationRequestDto = new PaymentCancellationRequestDto(
                paymentCancellationRequest.TransactionId
            );
            var result = await _cloudPaymentsApi.CancelPaymentAsync(_authHeader, paymentCancellationRequestDto);

            return Response<OperationResult>.Success(
                result.Success
                    ? OperationResult.Success()
                    : OperationResult.Failure(result.Message)
            );
        }

        private PaymentRequestDto TransformPaymentRequestToDto(PaymentRequest paymentRequest)
        {
            return new PaymentRequestDto(
                paymentRequest.Amount,
                _currencyCodeProvider.Provide(paymentRequest.Currency),
                paymentRequest.CardHolderIpAddress,
                paymentRequest.CardHolderName,
                paymentRequest.CardCryptogramPacket
            )
            {
                AccountId = paymentRequest.UserIdentifier,
                Description = paymentRequest.Description
            };
        }

        private PaymentCardInfo CreatePaymentCardInfoFromPaymentResponse(PaymentResponseDto paymentResponseDto)
        {
            return _paymentResponseToPaymentCardInfoMapper.Transform(paymentResponseDto);
        }
    }
}