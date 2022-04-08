using System;
using System.Collections.Generic;
using System.Linq;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Bussines;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Models.Transport;
using NasladdinPlace.CheckOnline.Builders.CheckOnline.Providers;
using Newtonsoft.Json.Linq;

namespace NasladdinPlace.Api.Tests.Utils.CheckOnline
{
	public sealed class FakeCheckOnlineRequestProvider : ICheckOnlineRequestProvider
	{
		public CheckOnlineResponseTransport SendRequest( CheckOnlineAuth authData, CheckOnlineRequestTransport requestData )
		{
			var now = DateTime.UtcNow;
			var docNumber = GenerateDocNumber( now );

			var result = new CheckOnlineResponseTransport
			{
				DocumentType = requestData.DocumentType,
				ClientId = "Fake ClientId",
				Date = CreateTransportDate( now ),
				Device =
					new CheckOnlineResponseTransportDevice {Address = "localhost", Name = Environment.MachineName},
				DeviceRegistrationNumber = "FAKE-XXX-XXX-XXX",
				DeviceSerialNumber = "FAKE-YYY-YYY-YYY",
				DocNumber = (int) docNumber,
				FiscalDocNumber = docNumber,
				FiscalSign = 0,
				FnSerialNumber = "FAKE-ZZZ-ZZZ-ZZZ",
				GrandTotal = requestData.Lines.Select( i => i.Price * i.Qty / 1000 ).Sum(),
				Path = "Fake Path",
				QrCode = "FAKE QR code",
				RequestId = requestData.RequestId,
				Response = new CheckOnlineResponseTransportResponse {Error = 0},
				Responses = new[]
				{
					new CheckOnlineResponseTransportResponses
					{
						Path = "/fr/api/v2/CloseDocument",
						Response = new CheckOnlineResponseTransportResponse
						{
							Error = 0, ReceiptFullText = "Fake check text"
						}
					}
				}
			};

			return result;
		}

		public CheckOnlineBatchResponseTransport SendRequest( CheckOnlineAuth authData, CheckOnlineBatchRequestTransport requestData )
		{
			var result = new CheckOnlineBatchResponseTransport
			{
				RequestId = requestData.RequestId,
				ErrorCode = 0,
				ErrorMessages = null,
				RequestError = null,
				Responses = requestData.Requests.Select( CreateResponse ).ToList()
			};

			return result;
		}

		private static CheckOnlineBatchResponseTransportCommand CreateResponse(
			CheckOnlineBatchRequestTransportCommand request )
		{
			var now = DateTime.UtcNow;
			var docNumber = GenerateDocNumber( now );

			return new CheckOnlineBatchResponseTransportCommand
			{
				Path = "Fake Path",
				ExchangeError = null,
				Response = JObject.FromObject(
					new CheckOnlineCorrectionResponseTransport
					{
						DocumentType = 0,
						ErrorCode = 0,
						FiscalDocument = new CheckOnlineCorrectionResponseTransportFiscalDocument
						{
							Tags = new List<CheckOnlineFiscalDocumentTag>
							{
								new CheckOnlineFiscalDocumentTag
								{
									TagId = 0,
									TagType = "tag",
									Value = 0
								}
							}
						},
						Date = CreateTransportDate( now ),
						DocumentNumber = (int)docNumber,
						FiscalDocNumber = docNumber,
						ErrorMessages = null,
						FiscalSign = 0
					} )
			};
		}

		private static long GenerateDocNumber( DateTime dtValue ) {
			return (dtValue - new DateTime( 2000, 1, 1, 0, 0, 0, DateTimeKind.Utc )).Ticks;
		}

		private static CheckOnlineResponseTransportDateTime CreateTransportDate( DateTime now ) {
			return new CheckOnlineResponseTransportDateTime {
				Date = new CheckOnlineResponseTransportDate {
					Day = now.Day,
					Month = now.Month,
					Year = now.Year
				},
				Time = new CheckOnlineResponseTransportTime {
					Hour = now.Hour,
					Minute = now.Minute,
					Second = now.Second
				}
			};
		}
	}
}