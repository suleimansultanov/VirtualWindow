using Microsoft.AspNetCore.Http;
using NasladdinPlace.Api.Client.Rest.Dtos.Account;
using NasladdinPlace.Api.Client.Rest.RequestExecutor.Models;
using NasladdinPlace.Dtos;
using NasladdinPlace.Dtos.Pos;
using NasladdinPlace.UI.Dtos.Good;
using NasladdinPlace.UI.Dtos.GoodImage;
using NasladdinPlace.UI.Dtos.Pos;
using NasladdinPlace.UI.Dtos.PosOperation;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Dtos.Catalog;
using NasladdinPlace.UI.Dtos.Catalog;
using PosTemperatureDto = NasladdinPlace.Dtos.Pos.PosTemperatureDto;

namespace NasladdinPlace.UI.Services.NasladdinApi
{
    public interface INasladdinApiClient
    {
        Task<RequestResponse<AuthPayloadDto>> LoginUserAsync(LoginDto loginDto);
        Task<RequestResponse<IEnumerable<PosDto>>> GetPointsOfSaleAsync();
        Task<RequestResponse<string>> GetPosLastReceivedWsMessageAsync(int posId);
        Task<RequestResponse<IEnumerable<PosDto>>> GetPointsOfSaleByRoleAsync();
        Task<RequestResponse<PosDto>> GetPosAsync(int posId);
        Task<RequestResponse<GoodDto>> GetGoodAsync(int goodId);
        Task<RequestResponse<int>> DeleteGoodAsync(int goodId);
        Task<RequestResponse<string>> AddGoodImageAsync(int goodId, IFormFile goodImageFile, int userId);
        Task<RequestResponse<int>> AddPosImageAsync(int posId, IFormFile imageFile);
        Task<RequestResponse<IEnumerable<ImageDto>>> GetPosImagesAsync(int posId);
        Task<RequestResponse<int>> DeletePosImageAsync(int posImageId);
        Task<RequestResponse<string>> UpdateAntennasOutputPowerAsync(int posId, PosAntennasOutputPowerDto outputPowerDto);
        Task<RequestResponse<string>> OpenLeftPosDoorAsync(int posId, PosOperationModeDto dto);
        Task<RequestResponse<string>> OpenRightPosDoorAsync(int posId, PosOperationModeDto dto);
        Task<RequestResponse<string>> ClosePosDoorsAsync(int posId);
        Task<RequestResponse<string>> SendPosContentRequestAsync(int posId);
        Task<RequestResponse<string>> ResetUserBalanceAsync(int userId);
        Task<RequestResponse<string>> RefundPaymentAsync(int posOperationId, int bankTransactionInfoId);
        Task<RequestResponse<IEnumerable<ScheduleDto>>> GetSchedulesAsync();
        Task<RequestResponse<byte>> RestartPromotionAgentAsync(PromotionDto promotionDto);
        Task<RequestResponse<byte>> StopPromotionAgentAsync(byte promotionType);
        Task<RequestResponse<string>> RefreshDisplayPageAsync(int posId);
        Task<RequestResponse<string>> RefreshAllDisplayPageAsync();
        Task<RequestResponse<IEnumerable<PosTemperatureDto>>> GetTemperaturesByPosIdAsync(int posId);
        Task<RequestResponse<string>> RequestPosLogsAsync(PosLogTypeDto logTypeDto);
        Task<RequestResponse<string>> UploadReportAsync(int type);
        Task<RequestResponse<string>> AddCategoryImageAsync(int goodCategoryId, IFormFile imageFile, int userId);
        Task<RequestResponse<IEnumerable<LabeledGoodWithImageDto>>> GetCategoryItemsAsync(CategoryItemsDto categoryItems);
        Task<RequestResponse<CatalogPointsOfSaleDto>> GetCatalogPointsOfSaleAsync(int page);
        Task<RequestResponse<List<PosContentDto>>> GetPosContentAsync(int posId, int page);
    }
}