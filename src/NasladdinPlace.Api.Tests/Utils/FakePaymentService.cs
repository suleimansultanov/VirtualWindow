using System.Threading.Tasks;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core.Services.Check.Helpers;
using NasladdinPlace.Payment.Models;
using NasladdinPlace.Payment.Services;

namespace NasladdinPlace.Api.Tests.Utils
{
	public class FakePaymentService : IPaymentService
	{
		private readonly IPaymentService _realPaymentService;

		private FakePaymentService(IPaymentService realPaymentService)
		{
			_realPaymentService = realPaymentService;
		}

		public Task<Response<PaymentResult>> MakePaymentAsync( PaymentRequest paymentRequest )
		{
			return _realPaymentService.MakePaymentAsync( paymentRequest );
		}

		public Task<Response<PaymentResult>> AuthPaymentAsync( PaymentRequest paymentRequest )
		{
			return _realPaymentService.AuthPaymentAsync( paymentRequest );
		}

		public Task<Response<PaymentResult>> Complete3DsPaymentAsync( Payment3DsCompletionRequest payment3DsCompletionRequest )
		{
			return _realPaymentService.Complete3DsPaymentAsync( payment3DsCompletionRequest );
		}

		public Task<Response<PaymentResult>> MakeRecurrentPaymentAsync( RecurrentPaymentRequest recurrentPaymentRequest )
		{
			return _realPaymentService.MakeRecurrentPaymentAsync( recurrentPaymentRequest );
		}

		public Task<Response<OperationResult>> MakeRefundAsync( RefundRequest refundRequest )
		{
			return CloudPaymentRepeater.MakeRefundAsync( _realPaymentService, refundRequest );
		}

		public Task<Response<OperationResult>> CancelPaymentAsync( PaymentCancellationRequest paymentCancellationRequest )
		{
			return _realPaymentService.CancelPaymentAsync( paymentCancellationRequest );
		}

        public Task<Response<string>> MakeFiscalizationAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Response<OperationResult>> GetFiscalCheckAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Response<OperationResult>> GetFiscalCheckStatusAsync()
        {
            throw new System.NotImplementedException();
        }

        public static IPaymentService Create( IPaymentService realPaymentService )
		{
			return new FakePaymentService( realPaymentService );
		}
	}
}