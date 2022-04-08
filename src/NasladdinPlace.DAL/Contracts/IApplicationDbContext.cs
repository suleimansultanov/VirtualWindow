using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Models.Goods;

namespace NasladdinPlace.DAL.Contracts
{
    public interface IApplicationDbContext
    {
        DbSet<ApplicationUser> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<Pos> PointsOfSale { get; set; }
        DbSet<Good> Goods { get; set; }
        DbSet<Country> Countries { get; set; }
        DbSet<City> Cities { get; set; }
        DbSet<PosOperation> PosOperations { get; set; }
        DbSet<Currency> Currencies { get; set; }
        DbSet<Maker> Makers { get; set; }
        DbSet<GoodCategory> GoodCategories { get; set; }
        DbSet<LabeledGood> LabeledGoods { get; set; }
        DbSet<GoodImage> GoodImages { get; set; }
        DbSet<PosImage> PosImages { get; set; }
        DbSet<CheckItem> CheckItems { get; set; }
        DbSet<UserFirebaseToken> UserFirebaseTokens { get; set; }
        DbSet<AllowedPosMode> AllowedPosModes { get; set; }
        DbSet<UserNotification>  UserNotifications { get; set; }
        DbSet<PromotionSetting> PromotionSettings { get; set; }
        DbSet<MediaContent> MediaContents { get; set; }
        DbSet<MediaContentToPosPlatform> MediaContentToPosPlatforms { get; set; }
        DbSet<ReportUploadingInfo> ReportsUploadingInfo { get; set; }
        DbSet<MessengerContact> MessengerContacts { get; set; }
        DbSet<FiscalizationInfo> FiscalizationInfos { get; set; }
        DbSet<PosAbnormalSensorMeasurement> PosAbnormalSensorMeasurements { get; set; }
        DbSet<PosLog> PosLogs { get; set; }
        DbSet<Discount> Discounts { get; set; }
        DbSet<DiscountRule> DiscountRules { get; set; }
        DbSet<DiscountRuleValue> DiscountRuleValues { get; set; }
        DbSet<PosTemperature> PosTemperatures { get; set; }
        DbSet<PosDoorsState> PosDoorsStates { get; set; }
        DbSet<PosDiscount> PosDiscounts { get; set; }
        DbSet<ConfigurationKey> ConfigurationKeys { get; set; }
        DbSet<ConfigurationValue> ConfigurationValues { get; set; }
        DbSet<FiscalizationCheckItem> FiscalizationCheckItems { get; set; }
        DbSet<PosOperationTransaction> PosOperationTransactions { get; set; }
        DbSet<BankTransactionInfoVersionTwo> BankTransactionInfosVersionTwo { get; set; }
        DbSet<FiscalizationInfoVersionTwo> FiscalizationInfosVersionTwo { get; set; }
        DbSet<PosOperationTransactionCheckItem> PosOperationTransactionCheckItems { get; set; }
        DbSet<PosScreenTemplate> PosScreenTemplates { get; set; }
        DbSet<DocumentGoodsMovingTableItem> DocumentGoodsMovingTableItems { get; set; }
        DbSet<DocumentGoodsMoving> DocumentsGoodsMoving { get; set; }
        DbSet<DocumentGoodsMovingLabeledGood> DocumentGoodsMovingLabeledGoods { get; set; }
    }
}