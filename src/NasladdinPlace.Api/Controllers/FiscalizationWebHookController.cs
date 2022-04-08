using CloudPaymentsClient.Domain.Factories.PaymentService;
using CloudPaymentsClient.Rest.Dtos.Fiscalization;
using CloudPaymentsClient.Rest.Dtos.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NasladdinPlace.Api.Constants;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Fiscalization.Services;
using NasladdinPlace.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;

namespace NasladdinPlace.Api.Controllers
{
    [Route(Routes.Api)]
    [AllowAnonymous]
    public class FiscalizationWebHookController : Controller
    {
        private const string ServiceId = "pk_d60a2fee7b77994ebee662cb2b6a6";
        private const string ServiceSecret = "bffbabb12e1eb01eed2096ec429778ba";
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IFiscalizationCheckPrinter _fiscalizationCheckPrinter;
        private readonly ILogger _logger;
        private readonly ICloudKassirService _cloudKassirService;

        public FiscalizationWebHookController(IUnitOfWorkFactory unitOfWorkFactory, ILogger logger, IFiscalizationCheckPrinter fiscalizationCheckPrinter)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (fiscalizationCheckPrinter == null)
                throw new ArgumentNullException(nameof(fiscalizationCheckPrinter));

            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;
            _fiscalizationCheckPrinter = fiscalizationCheckPrinter;

            var serviceInfo = new ServiceInfo(ServiceId, ServiceSecret);
            _cloudKassirService = new CloudPaymentsesServiceFactory(serviceInfo).CreateCloudKassirService();
        }

        [HttpPost]
        public async Task<ReceiptResultDto> Receipt()
        {
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                var body = reader.ReadToEnd();

                if (!IsRequestValid(body))
                    return ReceiptResult.Failure();

                var receipt = GetReceiptObject(body);

                if (receipt == null)
                    return ReceiptResult.Failure();

                using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
                {
                    var storedFiscalizationInfo = await unitOfWork.FiscalizationInfosV2.GetLastIncludePosOperationTransactionByRequestIdAsync(receipt.Id);

                    if (storedFiscalizationInfo == null)
                        return ReceiptResult.Failure();

                    receipt.DocumentInfo = _fiscalizationCheckPrinter.Print(receipt);

                    storedFiscalizationInfo.MarkAsSuccessReceipt(receipt);

                    await unitOfWork.CompleteAsync();
                }
            }

            return ReceiptResult.Success();
        }

        //public async Task<Response<string>> MakeFiscalization()
        //{

        //    var result = await _cloudKassirService.MakeFiscalizationAsync();

        //    using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
        //    {
        //        var fiscalizationInfos = new FiscalizationInfoVersionTwo(result.Result, 7722);
        //        unitOfWork.FiscalizationInfosV2.Add(fiscalizationInfos);

        //        await unitOfWork.CompleteAsync();
        //    }

        //    return result;

        //    return Response<string>.Success("");

        //}

        private bool IsRequestValid(string body)
        {
            var headerHmac = HttpContext.Request.Headers["Content-Hmac"];
            var bodyHmac = CreateToken(body, ServiceSecret);

            return headerHmac == bodyHmac;
        }

        private ReceiptDto GetReceiptObject(string body)
        {
            var qeryString = HttpUtility.ParseQueryString(body);
            var json = ToJson(qeryString);

            json = json.Replace("\"{", "{").Replace("}\"", "}").Replace("\\", "");

            ReceiptDto deserializedObject = null;

            try
            {
                deserializedObject = JsonConvert.DeserializeObject<ReceiptDto>(json);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deserialization object. Error {ex}");
            }

            return deserializedObject;
        }

        private static string ToJson(NameValueCollection collection)
        {
            var list = new Dictionary<string, string>();
            foreach (string key in collection.Keys)
            {
                if (key == "QrCodeUrl")
                {
                    var uri = new Uri(collection[key]);
                    var query = HttpUtility.ParseQueryString(uri.Query);

                    var result = query.Count > 0 ? query.Get(0) : string.Empty;
                    list.Add(key, result);

                    continue;
                }

                list.Add(key, collection[key]);
            }
            return JsonConvert.SerializeObject(list);
        }

        private static string CreateToken(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            var keyByte = encoding.GetBytes(secret);
            var messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                var hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

    }
}