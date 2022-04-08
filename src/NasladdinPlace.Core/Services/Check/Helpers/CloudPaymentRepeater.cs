using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Payment.Models;
using NasladdinPlace.Payment.Services;

namespace NasladdinPlace.Core.Services.Check.Helpers
{
	public static class CloudPaymentRepeater
	{
		private static readonly byte RepeatCount = 60;
		private static readonly int RepeatTimeout = 1000;

		public static Task<Response<OperationResult>> MakeRefundAsync( IPaymentService paymentService, RefundRequest request )
		{
			if ( paymentService == null )
				throw new ArgumentNullException( nameof( paymentService ) );
			if ( request == null )
				throw new ArgumentNullException( nameof( request ) );

			Task<Response<OperationResult>> result = null;

			for ( var attempt = 1; attempt <= RepeatCount; attempt++ ) {
				result = paymentService.MakeRefundAsync( request );
				var response = result.Result;

				if ( !response.IsSuccess || response.Status != ResponseStatus.Success )
					return result;

				if ( response.Result.IsSuccessful )
					return result;

				Debug.WriteLine(
					$"Payment service refund response attempt #{attempt}. IsSuccess: {response.IsSuccess}, Status: {response.Status}, " +
					$"Result.IsSuccessful: {response.Result.IsSuccessful}, Result.Error: {response.Result.Error}" );

				Thread.Sleep( RepeatTimeout );
			}

			return result;
		}
	}
}