using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.Api;
using NasladdinPlace.Api.Client.Rest.Constants;
using NasladdinPlace.Dtos;
using NasladdinPlace.Dtos.Catalog;
using NasladdinPlace.Dtos.Pos;
using NasladdinPlace.UI.Dtos.Catalog;
using NasladdinPlace.UI.Dtos.Good;
using NasladdinPlace.UI.Dtos.GoodImage;
using NasladdinPlace.UI.Dtos.Pos;
using NasladdinPlace.UI.Dtos.PosOperation;
using NasladdinPlace.UI.Dtos.User;
using Refit;
using PosTemperatureDto = NasladdinPlace.Dtos.Pos.PosTemperatureDto;

namespace NasladdinPlace.UI.Services.NasladdinApi
{
    public interface INasladdinApi : IApi
    {
        [Post("/api/plants/{posId}/rightDoor")]
        [Headers(Headers.Authorization)]
        Task<string> OpenRightPosDoor(
            int posId,
            [Body] PosOperationModeDto dto
        );

        [Post("/api/plants/{posId}/leftDoor")]
        [Headers(Headers.Authorization)]
        Task<string> OpenLeftPosDoor(int posId, [Body] PosOperationModeDto dto);

        [Delete("/api/plants/{posId}/doors")]
        [Headers(Headers.Authorization)]
        Task<string> ClosePosDoors(int posId
        );

        [Delete("/api/goods/{goodId}")]
        [Headers(Headers.Authorization)]
        Task<int> DeleteGood(
            int goodId
        );

        [Get("/api/plants/")]
        [Headers(Headers.Authorization)]
        Task<IEnumerable<PosDto>> GetPointsOfSale();

        [Get("/api/plants/{posId}/lastReceivedWsMessage")]
        [Headers(Headers.Authorization)]
        Task<string> GetPosLastReceivedWsMessage(int posId);

        [Get("/api/pointsOfSale/byRole")]
        [Headers(Headers.Authorization)]
        Task<IEnumerable<PosDto>> GetPointsOfSaleByRole();

        [Post("/api/pointsOfSale/{posId}/display/reloading")]
        [Headers(Headers.Authorization)]
        Task<string> RefreshDisplayPage(int posId);

        [Post("/api/pointsOfSale/all/display/reloading")]
        [Headers(Headers.Authorization)]
        Task<string> RefreshAllDisplayPage();

        [Get("/api/plants/{posId}")]
        [Headers(Headers.Authorization)]
        Task<PosDto> GetPos(
            int posId
        );

        [Post("/api/plants/{posId}/antennasOutputPower")]
        [Headers(Headers.Authorization)]
        Task<string> UpdateAntennasOutputPower(
            int posId,
            [Body] PosAntennasOutputPowerDto outputPowerDto
        );

        [Get("/api/users/")]
        [Headers(Headers.Authorization)]
        Task<IEnumerable<UserDto>> GetUsers();

        [Get("/api/goods/{id}")]
        [Headers(Headers.Authorization)]
        Task<GoodDto> GetGood(
            [AliasAs("id")] int goodId
        );

        [Multipart]
        [Post("/api/goods/{goodId}/images/")]
        [Headers(Headers.Authorization)]
        Task<string> UploadGoodImage(
            int goodId,
            [AliasAs("goodImageFile")] StreamPart goodImageFileStream,
            int userId
        );

        [Multipart]
        [Post("/api/goodCategories/{goodCategoryId}/image")]
        [Headers(Headers.Authorization)]
        Task<string> UploadGoodCategoryImage(
            int goodCategoryId,
            [AliasAs("goodCategoryImageFile")] StreamPart goodCategoryImageFileStream, 
            int userId
        );

        [Get("/api/pointsOfSale/{posId}/images/")]
        [Headers(Headers.Authorization)]
        Task<IEnumerable<ImageDto>> GetPosImages(
            int posId
        );

        [Multipart]
        [Post("/api/pointsOfSale/{posId}/images/")]
        [Headers(Headers.Authorization)]
        Task<int> UploadPosImage(
            int posId,
            [AliasAs("imageFile")] StreamPart imageFileStream
        );

        [Delete("/api/pointsOfSale/{posId}/images/{posImageId}")]
        [Headers(Headers.Authorization)]
        Task<int> DeletePosImage(
            int posId,
            int posImageId
        );

        [Get("/api/plants/{posId}/content")]
        [Headers(Headers.Authorization)]
        Task<string> SendPosContentRequest(
            int posId
        );

        [Delete("/api/users/{userId}/purchases/unfinished")]
        [Headers(Headers.Authorization)]
        Task<string> ResetUserBalance(
            int userId
        );

        [Patch("/api/purchases/{userShopTransactionId}/bankTransactionInfos/{bankTransactionInfoId}/refund")]
        [Headers(Headers.Authorization)]
        Task<string> RefundPayment(
            int userShopTransactionId,
            int bankTransactionInfoId
        );

        [Get("/api/jobsScheduler/")]
        [Headers(Headers.Authorization)]
        Task<IEnumerable<ScheduleDto>> GetSchedules(
        );

        [Post("/api/jobsScheduler/promotionAgentRestart")]
        [Headers(Headers.Authorization)]
        Task<byte> RestartPromotionAgent(
            [Body] PromotionDto promotionDto
        );

        [Delete("/api/jobsScheduler/promotionAgentStop/{promotionType}")]
        [Headers(Headers.Authorization)]
        Task<byte> StopPromotionAgent(
            byte promotionType
        );

        [Get("/api/plants/{posId}/temperatureDetails")]
        [Headers(Headers.Authorization)]
        Task<IEnumerable<PosTemperatureDto>> GetTemperaturesByPosId(
            int posId
        );

        [Post("/api/logs")]
        [Headers(Headers.Authorization)]
        Task<string> RequestPosLogs(
            [Body] PosLogTypeDto logTypeDto
        );

        [Post("/api/SpreadsheetsUploader/{type}")]
        [Headers(Headers.Authorization)]
        Task<string> Upload(
            int type
        );

        [Post("/api/catalog/categoryItems")]
        [Headers(Headers.Authorization)]
        Task<IEnumerable<LabeledGoodWithImageDto>> GetCategoryItems(
            [Body] CategoryItemsDto categoryItems
        );

        [Get("/api/catalog/pointsOfSale")]
        [Headers(Headers.Authorization)]
        Task<CatalogPointsOfSaleDto> GetCatalogPointsOfSale(
            int page
        );

        [Get("/api/catalog/posContent")]
        [Headers(Headers.Authorization)]
        Task<List<PosContentDto>> GetPosContent(
            int posId, int page
        );
    }
}
