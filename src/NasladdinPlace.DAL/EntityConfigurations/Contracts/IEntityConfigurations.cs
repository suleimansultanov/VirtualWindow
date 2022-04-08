using Microsoft.AspNetCore.Identity;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.DAL.Entities;

namespace NasladdinPlace.DAL.EntityConfigurations.Contracts
{
    public interface IEntityConfigurations
    {
        IEntityConfiguration<ApplicationUser> ApplicationUserConfiguration { get; }
        IEntityConfiguration<Role> RoleConfiguration { get; }
        IEntityConfiguration<UserRole> IdentityUserRoleConfiguration { get; }
        IEntityConfiguration<IdentityUserLogin<int>> IdentityUserLoginConfiguration { get; }
        IEntityConfiguration<IdentityRoleClaim<int>> IdentityRoleClaimConfiguration { get; }
        IEntityConfiguration<IdentityUserClaim<int>> IdentityUserClaimConfiguration { get; }
        IEntityConfiguration<IdentityUserToken<int>> IdentityUserTokenConfiguration { get; }
        IEntityConfiguration<Currency> CurrencyConfiguration { get; set; }
        IEntityConfiguration<Country> CountryConfiguration { get; set; }
        IEntityConfiguration<Maker> MakerConfiguration { get; set; }
        IEntityConfiguration<GoodCategory> GoodCategoryConfiguration { get; set; }
        IEntityConfiguration<City> CityConfiguration { get; set; }
        IEntityConfiguration<Pos> PosConfiguration { get; set; }
        IEntityConfiguration<PosOperation> PosOperationConfiguration { get; set; }
        IEntityConfiguration<Good> GoodConfiguration { get; set; }
        IEntityConfiguration<LabeledGood> LabeledGoodConfiguration { get; set; }
        IEntityConfiguration<GoodImage> GoodImageConfiguration { get; set; }
        IEntityConfiguration<PosImage> PosImageConfiguration { get; }
        IEntityConfiguration<BankTransactionInfo> BankTransactionInfoConfiguration { get; }
        IEntityConfiguration<FeedbackEntity> FeedbackEntityConfiguration { get; }
        IEntityConfiguration<UserBonusPoint> UserBonusPointConfiguration { get; }
        IEntityConfiguration<PaymentCard> PaymentCardConfiguration { get; }
        IEntityConfiguration<CheckItem> CheckItemConfiguration { get; }
        IEntityConfiguration<UserFirebaseToken> UserFirebaseTokenConfiguration { get; }
        IEntityConfiguration<PromotionLog> PromotionLogConfiguration { get; }
        IEntityConfiguration<AllowedPosMode> AllowedPosModeConfiguration { get; }
        IEntityConfiguration<RolePermittedAppFeature> RolePermittedAppFeatureConfiguration { get; }
        IEntityConfiguration<UserNotification> UserNotificationConfiguration { get; }
        IEntityConfiguration<PromotionSetting> PromotionSettingConfiguration { get; }
        IEntityConfiguration<MediaContent> MediaContentConfiguration { get; }
        IEntityConfiguration<MediaContentToPosPlatform> MediaContentToPosPlatformConfiguration { get; }
        IEntityConfiguration<PosMediaContent> PosMediaContentConfiguration { get; }
        IEntityConfiguration<ReportUploadingInfo> ReportUploadingInfoConfiguration { get; }
        IEntityConfiguration<MessengerContact> MessengerContactConfiguration { get; }
        IEntityConfiguration<FiscalizationInfo> FiscalizationInfoConfiguration { get; }
        IEntityConfiguration<LabeledGoodTrackingRecord> LabeledGoodTrackingRecordConfiguration { get; }
        IEntityConfiguration<PosAbnormalSensorMeasurement> PosAbnormalSensorMeasurementConfiguration { get; }
        IEntityConfiguration<PosLog> PosLogConfiguration { get; }
        IEntityConfiguration<Discount> DiscountConfiguration { get; }
        IEntityConfiguration<DiscountRule> DiscountRuleConfiguration { get; }
        IEntityConfiguration<DiscountRuleValue> DiscountRuleValueConfiguration { get; }
        IEntityConfiguration<PosDiscount> PosDiscountConfiguration { get; }
        IEntityConfiguration<PosTemperature> PosTemperatureConfiguration { get; }
        IEntityConfiguration<PosDoorsState> PosDoorsStateConfiguration { get; }
        IEntityConfiguration<CheckItemAuditRecord> CheckItemAuditRecordConfiguration { get; }
        IEntityConfiguration<ConfigurationValue> ConfigurationValueConfiguration { get; }
        IEntityConfiguration<ConfigurationKey> ConfigurationKeyConfiguration { get; }
        IEntityConfiguration<FiscalizationCheckItem> FiscalizationCheckItemConfiguration { get; }
        IEntityConfiguration<PosOperationTransaction> PosOperationTransactionConfiguration { get; }
        IEntityConfiguration<BankTransactionInfoVersionTwo> BankTransactionInfoVersionTwoConfiguration { get; }
        IEntityConfiguration<FiscalizationInfoVersionTwo> FiscalizationInfoVersionTwoConfiguration { get; }
        IEntityConfiguration<PosOperationTransactionCheckItem> PosOperationTransactionCheckItemConfiguration { get; }
        IEntityConfiguration<PosScreenTemplate> PosScreenTemplateConfiguration { get; }
        IEntityConfiguration<AppFeatureItemsToRole> AppFeaturesToRoleConfiguration { get; }
        IEntityConfiguration<PointsOfSaleToRole> PointsOfSaleToRoleConfiguration { get; }
        IEntityConfiguration<AppFeatureItem> AppFeatureItemConfiguration { get; }
        IEntityConfiguration<DocumentGoodsMovingTableItem> DocumentGoodsMovingTableItemConfiguration { get; }
        IEntityConfiguration<DocumentGoodsMoving> DocumentGoodsMovingConfiguration { get; }
        IEntityConfiguration<DocumentGoodsMovingLabeledGood> DocumentGoodsMovingLabeledGoodConfiguration { get; }
    }
}