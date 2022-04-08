using System;
using CloudPaymentsClient.Domain.Factories.CurrencyCode;
using CloudPaymentsClient.Domain.Helpers.AuthHeader;
using CloudPaymentsClient.Domain.Services;
using CloudPaymentsClient.Rest.Api;
using CloudPaymentsClient.Rest.Mappers;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Fiscalization.Services;
using NasladdinPlace.Payment.Services;
using Refit;

namespace CloudPaymentsClient.Domain.Factories.PaymentService
{
    public class CloudPaymentsesServiceFactory : ICloudPaymentsServiceFactory
    {
        private readonly IPaymentServiceAuthHeaderMaker _paymentServiceAuthHeaderMaker;
        private readonly ICloudPaymentsApi _cloudPaymentsApi;
        private readonly ICurrencyCodeProvider _currencyCodeProvider;
        private readonly IPaymentResponseToPaymentCardInfoMapper _paymentResponseToPaymentCardInfoMapper;
        private readonly string _authHeader;

        public CloudPaymentsesServiceFactory(ServiceInfo serviceInfo)
        {
            if (serviceInfo == null)
                throw new ArgumentNullException(nameof(serviceInfo));
            
            _paymentServiceAuthHeaderMaker = new BasicPaymentServiceAuthHeaderMaker();
            _cloudPaymentsApi = RestService.For<ICloudPaymentsApi>("https://api.cloudpayments.ru");
            _currencyCodeProvider = new CurrencyCodeProvider();
            _paymentResponseToPaymentCardInfoMapper = new PaymentResponseToPaymentCardInfoMapper();
            _authHeader = _paymentServiceAuthHeaderMaker.Make(serviceInfo);
        }

        public IPaymentService CreatePaymentService()
        {
            return new CloudPaymentsService(
                _authHeader,
                _cloudPaymentsApi,
                _currencyCodeProvider,
                _paymentResponseToPaymentCardInfoMapper
            );
        }

        public ICloudKassirService CreateCloudKassirService()
        {
            return new CloudKassirService(_authHeader, _cloudPaymentsApi);
        }
    }
}