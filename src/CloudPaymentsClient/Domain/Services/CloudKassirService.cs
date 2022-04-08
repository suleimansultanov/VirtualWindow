using CloudPaymentsClient.Rest.Api;
using CloudPaymentsClient.Rest.Dtos.Fiscalization;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Fiscalization.Models;
using NasladdinPlace.Fiscalization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudPaymentsClient.Domain.Services
{
    public class CloudKassirService : CloudPaymentsBaseService, ICloudKassirService
    {
        private readonly string _authHeader;
        private readonly ICloudPaymentsApi _cloudPaymentsApi;

        public CloudKassirService(string authHeader, ICloudPaymentsApi cloudPaymentsApi)
        {
            if (string.IsNullOrWhiteSpace(authHeader))
                throw new ArgumentNullException(nameof(authHeader));
            if (cloudPaymentsApi == null)
                throw new ArgumentNullException(nameof(cloudPaymentsApi));

            _authHeader = authHeader;
            _cloudPaymentsApi = cloudPaymentsApi;
        }

        public Task<Response<FiscalizationResult>> MakeFiscalizationAsync(FiscalizationRequest fiscalizationRequest)
        {
            return PerformRequest(() => MakeFiscalizationAuxAsync(fiscalizationRequest));
        }

        public Task<Response<CheckInfoResult>> GetFiscalCheckAsync(CheckInfoRequest checkInfoRequest)
        {
            return PerformRequest(() => GetFiscalCheckAuxAsync(checkInfoRequest));
        }

        public Task<Response<string>> GetFiscalCheckStatusAsync(CheckInfoRequest checkInfoRequest)
        {
            return PerformRequest(() => GetFiscalCheckStatusAuxAsync(checkInfoRequest));
        }

        private async Task<Response<FiscalizationResult>> MakeFiscalizationAuxAsync(FiscalizationRequest fiscalizationRequest)
        {
            var fiscalDataDto = TransformFiscalizationRequestToDto(fiscalizationRequest);
            var result = await _cloudPaymentsApi.MakeFiscalizationAsync(_authHeader, fiscalDataDto);
            return result.Success
                ? Response<FiscalizationResult>.Success(FiscalizationResult.Fiscalized(result.Model.Id))
                : Response<FiscalizationResult>.Failure(result.Message);
        }

        private async Task<Response<CheckInfoResult>> GetFiscalCheckAuxAsync(CheckInfoRequest checkInfoRequest)
        {
            var checkInfoDto = new CheckInfoDto { Id = checkInfoRequest.Id };
            var result = await _cloudPaymentsApi.GetFiscalCheckAsync(_authHeader, checkInfoDto);

            if (!result.Success)
                return Response<CheckInfoResult>.Failure(result.Message);

            var checkInfoReult = ProcessCheckInfoResponse(result.Model);

            return Response<CheckInfoResult>.Success(checkInfoReult);
        }

        private async Task<Response<string>> GetFiscalCheckStatusAuxAsync(CheckInfoRequest checkInfoRequest)
        {
            var checkInfoDto = new CheckInfoDto { Id = checkInfoRequest.Id };
            var result = await _cloudPaymentsApi.GetFiscalCheckStatusAsync(_authHeader, checkInfoDto);
            return result.Success
                ? Response<string>.Success(result.Model)
                : Response<string>.Failure(result.Model);
        }

        private FiscalDataDto TransformFiscalizationRequestToDto(FiscalizationRequest fiscalizationRequest)
        {
            var requestAmounts = fiscalizationRequest.CustomerReceipt.Amounts;
            var requestFiscalItems = fiscalizationRequest.CustomerReceipt.Items;
            var requestTaxationSystem = fiscalizationRequest.CustomerReceipt.TaxationSystem;

            var amountsDto = new AmountsDto(requestAmounts.Electronic, requestAmounts.AdvancePayment, requestAmounts.Credit, requestAmounts.Provision);
            var fiscalItemsDto = requestFiscalItems
                .Select(fi => new FiscalItemDto(fi.Label, fi.Price, fi.Quantity, fi.Vat))
                .ToList();
            var customerReceiptDto = new CustomerReceiptDto(fiscalItemsDto, requestTaxationSystem, amountsDto);

            return new FiscalDataDto(fiscalizationRequest.Inn, fiscalizationRequest.Type, customerReceiptDto);
        }

        private CheckInfoResult ProcessCheckInfoResponse(CheckInfoResponseDto checkInfoResponseDto)
        {
            var amountsDto = checkInfoResponseDto.Amounts;
            var fiscalItemsDto = checkInfoResponseDto.Items;
            var additionalDataDto = checkInfoResponseDto.AdditionalData;

            var amounts = amountsDto?.Electronic == null ? null : Amounts.CreateForElectronic(amountsDto.Electronic);
            var fiscalItems = fiscalItemsDto
                .Select(fi => new FiscalItem(fi.Label, fi.Price, fi.Quantity, fi.Vat))
                .ToList();
            var additionalData = TransformAdditionalDataDtoToBusinessModel(additionalDataDto);

            return new CheckInfoResult(
                fiscalItems,
                checkInfoResponseDto.TaxationSystem,
                amounts,
                additionalData)
            {
                Email = checkInfoResponseDto.Email,
                Phone = checkInfoResponseDto.Phone,
                IsBso = checkInfoResponseDto.IsBso
            };
        }

        private AdditionalData TransformAdditionalDataDtoToBusinessModel(AdditionalDataDto additionalDataDto)
        {
            return new AdditionalData
            {
                Id = additionalDataDto.Id,
                AccountId = additionalDataDto.AccountId,
                DateTime = additionalDataDto.DateTime,
                Type = additionalDataDto.Type,
                DocumentNumber = additionalDataDto.DocumentNumber,
                QrCodeUrl = additionalDataDto.QrCodeUrl,
                FiscalNumber = additionalDataDto.FiscalNumber,
                Amount = additionalDataDto.Amount,
                FiscalSign = additionalDataDto.FiscalSign,
                DeviceNumber = additionalDataDto.DeviceNumber,
                TransactionId = additionalDataDto.TransactionId,
                CalculationPlace = additionalDataDto.CalculationPlace,
                RegNumber = additionalDataDto.RegNumber,
                SessionNumber = additionalDataDto.SessionNumber,
                Ofd = additionalDataDto.Ofd,
                CashierName = additionalDataDto.CashierName,
                InvoiceId = additionalDataDto.InvoiceId,
                OfdReceiptUrl = additionalDataDto.OfdReceiptUrl,
                OrganizationInn = additionalDataDto.OrganizationInn,
                SenderEmail = additionalDataDto.SenderEmail,
                SessionCheckNumber = additionalDataDto.SessionCheckNumber,
                SettlePlace = additionalDataDto.SettlePlace
            };
        }

        //TODO: Delete this when web hook will implemented
        private FiscalDataDto CreateStubPaymentFiscalDataDto()
        {
            var customerReceiptDto = new CustomerReceiptDto
            {
                Items = new List<FiscalItemDto>
                { new FiscalItemDto
                    {
                        Label = "Блинчики с мясом",
                        Price = 100,
                        Quantity = 1,
                        Amount = 100,
                        Vat = VatValues.P0,
                        //Method = 0,
                        //Object = 0,
                        //MeasurementUnit = "nn"
                    },
                    new FiscalItemDto
                    {
                        Label = "Блинчики с мясом",
                        Price = 100,
                        Quantity = 1,
                        Amount = 100,
                        Vat = VatValues.P0,
                        //Method = 0,
                        //Object = 0,
                        //MeasurementUnit = "nn"
                    },
                    new FiscalItemDto
                    {
                        Label = "Сэндвич с тунцом",
                        Price = 150,
                        Quantity = 1,
                        Amount = 150,
                        Vat = VatValues.P0,
                        //Method = 0,
                        //Object = 0,
                        //MeasurementUnit = "nn"
                    },
                },
                //CalculationPlace = "www.my.ru",
                TaxationSystem = TaxationSystem.Common,
                //CustomerInn = "1231231234",
                //IsBso = false,
                Amounts = new AmountsDto
                {
                    Electronic = 350,
                    AdvancePayment = 0,
                    Credit = 0,
                    Provision = 0
                },
                //Email = "evg.moskalenko@gmail.com"

            };

            return new FiscalDataDto
            {
                Inn = "5048047172",
                Type = "Income",
                CustomerReceipt = customerReceiptDto
            };
        }
    }
}
