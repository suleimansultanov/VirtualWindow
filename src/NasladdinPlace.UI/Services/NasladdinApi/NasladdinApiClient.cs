using Microsoft.AspNetCore.Http;
using NasladdinPlace.Api.Client.Rest.Api;
using NasladdinPlace.Api.Client.Rest.Client.Contracts;
using NasladdinPlace.Api.Client.Rest.Dtos.Account;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;
using NasladdinPlace.Dtos;
using NasladdinPlace.Dtos.Pos;
using NasladdinPlace.UI.Dtos.Good;
using NasladdinPlace.UI.Dtos.GoodImage;
using NasladdinPlace.UI.Dtos.Pos;
using NasladdinPlace.UI.Dtos.PosOperation;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Dtos.Catalog;
using NasladdinPlace.UI.Dtos.Catalog;
using PosTemperatureDto = NasladdinPlace.Dtos.Pos.PosTemperatureDto;

namespace NasladdinPlace.UI.Services.NasladdinApi
{
    public class NasladdinApiClient : INasladdinApiClient
    {
        private readonly IRestClient _restClient;

        public NasladdinApiClient(IRestClient restClient)
        {
            if (restClient == null)
                throw new ArgumentNullException(nameof(restClient));

            _restClient = restClient;
        }

        public Task<RequestResponse<AuthPayloadDto>> LoginUserAsync(LoginDto loginDto)
        {
            return _restClient.PerformRequestAsync((IAuthApi api) => api.LoginUserAsync(loginDto));
        }

        public Task<RequestResponse<IEnumerable<PosDto>>> GetPointsOfSaleAsync()
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.GetPointsOfSale()
            );
        }

        public Task<RequestResponse<string>> GetPosLastReceivedWsMessageAsync(int posId)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.GetPosLastReceivedWsMessage(posId)
            );
        }

        public Task<RequestResponse<IEnumerable<PosDto>>> GetPointsOfSaleByRoleAsync()
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.GetPointsOfSaleByRole()
            );
        }

        public Task<RequestResponse<PosDto>> GetPosAsync(int posId)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.GetPos(posId)
            );
        }

        public Task<RequestResponse<GoodDto>> GetGoodAsync(int goodId)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.GetGood(goodId)
            );
        }

        public Task<RequestResponse<int>> DeleteGoodAsync(int goodId)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.DeleteGood(goodId)
            );
        }

        public Task<RequestResponse<string>> AddGoodImageAsync(int goodId, IFormFile goodImageFile, int userId)
        {
            return _restClient.PerformRequestAsync((INasladdinApi api) =>
            {
                var goodImageFileStream = new StreamPart(goodImageFile.OpenReadStream(), goodImageFile.FileName, "image/*");
                return api.UploadGoodImage(goodId, goodImageFileStream, userId);
            });
        }

        public Task<RequestResponse<string>> AddCategoryImageAsync(int goodCategoryId, IFormFile imageFile, int userId)
        {
            return _restClient.PerformRequestAsync((INasladdinApi api) =>
            {
                var goodCategoryImageFileStream = new StreamPart(imageFile.OpenReadStream(), imageFile.FileName, "image/*");
                return api.UploadGoodCategoryImage(goodCategoryId, goodCategoryImageFileStream, userId);
            });
        }

        public Task<RequestResponse<int>> AddPosImageAsync(int posId, IFormFile imageFile)
        {
            return _restClient.PerformRequestAsync((INasladdinApi api) =>
            {
                var goodImageFileStream = new StreamPart(imageFile.OpenReadStream(), imageFile.FileName, "image/*");
                return api.UploadPosImage(posId, goodImageFileStream);
            });
        }

        public Task<RequestResponse<IEnumerable<ImageDto>>> GetPosImagesAsync(int posId)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.GetPosImages(posId)
            );
        }

        public Task<RequestResponse<int>> DeletePosImageAsync(int posImageId)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.DeletePosImage(0, posImageId)
            );
        }

        public Task<RequestResponse<string>> UpdateAntennasOutputPowerAsync(int posId,
            PosAntennasOutputPowerDto outputPowerDto)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.UpdateAntennasOutputPower(posId, outputPowerDto)
            );
        }

        public Task<RequestResponse<string>> OpenLeftPosDoorAsync(int posId, PosOperationModeDto dto)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.OpenLeftPosDoor(posId, dto)
            );
        }

        public Task<RequestResponse<string>> OpenRightPosDoorAsync(int posId, PosOperationModeDto dto)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.OpenRightPosDoor(posId, dto)
            );
        }

        public Task<RequestResponse<string>> ClosePosDoorsAsync(int posId)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.ClosePosDoors(posId)
            );
        }

        public Task<RequestResponse<string>> SendPosContentRequestAsync(int posId)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.SendPosContentRequest(posId)
            );
        }

        public Task<RequestResponse<string>> ResetUserBalanceAsync(int userId)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.ResetUserBalance(userId)
            );
        }

        public Task<RequestResponse<string>> RefundPaymentAsync(int posOperationId, int bankTransactionInfoId)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.RefundPayment(posOperationId, bankTransactionInfoId)
            );
        }

        public Task<RequestResponse<IEnumerable<ScheduleDto>>> GetSchedulesAsync()
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.GetSchedules()
            );
        }

        public Task<RequestResponse<byte>> RestartPromotionAgentAsync(PromotionDto promotionDto)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.RestartPromotionAgent(promotionDto)
            );
        }

        public Task<RequestResponse<byte>> StopPromotionAgentAsync(byte promotionType)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.StopPromotionAgent(promotionType)
            );
        }

        public Task<RequestResponse<string>> RefreshDisplayPageAsync(int posId)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.RefreshDisplayPage(posId)
            );
        }

        public Task<RequestResponse<string>> RefreshAllDisplayPageAsync()
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.RefreshAllDisplayPage()
            );
        }

        public Task<RequestResponse<IEnumerable<PosTemperatureDto>>> GetTemperaturesByPosIdAsync(int posId)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.GetTemperaturesByPosId(posId)
            );
        }

        public Task<RequestResponse<string>> RequestPosLogsAsync(PosLogTypeDto logTypeDto)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.RequestPosLogs(logTypeDto)
            );
        }

        public Task<RequestResponse<string>> UploadReportAsync(int type)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.Upload(type)
            );
        }

        public Task<RequestResponse<IEnumerable<LabeledGoodWithImageDto>>> GetCategoryItemsAsync(CategoryItemsDto categoryItems)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.GetCategoryItems(categoryItems)
            );
        }

        public Task<RequestResponse<CatalogPointsOfSaleDto>> GetCatalogPointsOfSaleAsync(int page)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.GetCatalogPointsOfSale(page)
            );
        }

        public Task<RequestResponse<List<PosContentDto>>> GetPosContentAsync(int posId, int page)
        {
            return _restClient.PerformRequestAsync(
                (INasladdinApi api) => api.GetPosContent(posId, page)
            );
        }

    }
}

