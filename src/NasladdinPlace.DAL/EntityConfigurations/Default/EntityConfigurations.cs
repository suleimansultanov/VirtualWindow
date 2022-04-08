using Microsoft.AspNetCore.Identity;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.DAL.Entities;
using NasladdinPlace.DAL.EntityConfigurations.Contracts;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class EntityConfigurations : IEntityConfigurations
    {
        public IEntityConfiguration<ApplicationUser> ApplicationUserConfiguration { get; }
        public IEntityConfiguration<Role> RoleConfiguration { get; }
        public IEntityConfiguration<UserRole> IdentityUserRoleConfiguration { get; }
        public IEntityConfiguration<IdentityUserLogin<int>> IdentityUserLoginConfiguration { get; }
        public IEntityConfiguration<IdentityRoleClaim<int>> IdentityRoleClaimConfiguration { get; }
        public IEntityConfiguration<IdentityUserClaim<int>> IdentityUserClaimConfiguration { get; }
        public IEntityConfiguration<IdentityUserToken<int>> IdentityUserTokenConfiguration { get; }
        public IEntityConfiguration<Currency> CurrencyConfiguration { get; set; }
        public IEntityConfiguration<Country> CountryConfiguration { get; set; }
        public IEntityConfiguration<Maker> MakerConfiguration { get; set; }
        public IEntityConfiguration<GoodCategory> GoodCategoryConfiguration { get; set; }
        public IEntityConfiguration<City> CityConfiguration { get; set; }
        public IEntityConfiguration<Pos> PosConfiguration { get; set; }
        public IEntityConfiguration<PosOperation> PosOperationConfiguration { get; set; }
        public IEntityConfiguration<Good> GoodConfiguration { get; set; }
        public IEntityConfiguration<LabeledGood> LabeledGoodConfiguration { get; set; }
        public IEntityConfiguration<GoodImage> GoodImageConfiguration { get; set; }
        public IEntityConfiguration<PosImage> PosImageConfiguration { get; }
        public IEntityConfiguration<BankTransactionInfo> BankTransactionInfoConfiguration { get; }
        public IEntityConfiguration<FeedbackEntity> FeedbackEntityConfiguration { get; }
        public IEntityConfiguration<UserBonusPoint> UserBonusPointConfiguration { get; }
        public IEntityConfiguration<PaymentCard> PaymentCardConfiguration { get; }
        public IEntityConfiguration<CheckItem> CheckItemConfiguration { get; }
        public IEntityConfiguration<UserFirebaseToken> UserFirebaseTokenConfiguration { get; }
        public IEntityConfiguration<PromotionLog> PromotionLogConfiguration { get; }
        public IEntityConfiguration<AllowedPosMode> AllowedPosModeConfiguration { get; }
        public IEntityConfiguration<RolePermittedAppFeature> RolePermittedAppFeatureConfiguration { get; }
        public IEntityConfiguration<UserNotification> UserNotificationConfiguration { get; }
        public IEntityConfiguration<PromotionSetting> PromotionSettingConfiguration { get; }
        public IEntityConfiguration<MediaContent> MediaContentConfiguration { get; set; }
        public IEntityConfiguration<MediaContentToPosPlatform> MediaContentToPosPlatformConfiguration { get; set; }
        public IEntityConfiguration<PosMediaContent> PosMediaContentConfiguration { get; set; }
        public IEntityConfiguration<ReportUploadingInfo> ReportUploadingInfoConfiguration { get; }
        public IEntityConfiguration<MessengerContact> MessengerContactConfiguration { get; }
        public IEntityConfiguration<FiscalizationInfo> FiscalizationInfoConfiguration { get; }
        public IEntityConfiguration<LabeledGoodTrackingRecord> LabeledGoodTrackingRecordConfiguration { get; }
        public IEntityConfiguration<PosAbnormalSensorMeasurement> PosAbnormalSensorMeasurementConfiguration { get; }
        public IEntityConfiguration<PosLog> PosLogConfiguration { get; }
        public IEntityConfiguration<Discount> DiscountConfiguration { get; }
        public IEntityConfiguration<DiscountRule> DiscountRuleConfiguration { get; }
        public IEntityConfiguration<DiscountRuleValue> DiscountRuleValueConfiguration { get; }
        public IEntityConfiguration<PosDiscount> PosDiscountConfiguration { get; }
        public IEntityConfiguration<PosTemperature> PosTemperatureConfiguration { get; }
        public IEntityConfiguration<PosDoorsState> PosDoorsStateConfiguration { get; }
        public IEntityConfiguration<CheckItemAuditRecord> CheckItemAuditRecordConfiguration { get; }
        public IEntityConfiguration<ConfigurationValue> ConfigurationValueConfiguration { get; }
        public IEntityConfiguration<ConfigurationKey> ConfigurationKeyConfiguration { get; }
        public IEntityConfiguration<FiscalizationCheckItem> FiscalizationCheckItemConfiguration { get; }
        public IEntityConfiguration<PosOperationTransaction> PosOperationTransactionConfiguration { get; }
        public IEntityConfiguration<BankTransactionInfoVersionTwo> BankTransactionInfoVersionTwoConfiguration { get; }
        public IEntityConfiguration<FiscalizationInfoVersionTwo> FiscalizationInfoVersionTwoConfiguration { get; }
        public IEntityConfiguration<PosOperationTransactionCheckItem> PosOperationTransactionCheckItemConfiguration { get; }
        public IEntityConfiguration<PosScreenTemplate> PosScreenTemplateConfiguration { get; }
        public IEntityConfiguration<AppFeatureItemsToRole> AppFeaturesToRoleConfiguration { get; }
        public IEntityConfiguration<PointsOfSaleToRole> PointsOfSaleToRoleConfiguration { get; }
        public IEntityConfiguration<AppFeatureItem> AppFeatureItemConfiguration { get; set; }
        public IEntityConfiguration<DocumentGoodsMovingTableItem> DocumentGoodsMovingTableItemConfiguration { get; }
        public IEntityConfiguration<DocumentGoodsMoving> DocumentGoodsMovingConfiguration { get; }
        public IEntityConfiguration<DocumentGoodsMovingLabeledGood> DocumentGoodsMovingLabeledGoodConfiguration { get; }

        public EntityConfigurations()
        {
            ApplicationUserConfiguration = new ApplicationUserConfiguration();
            RoleConfiguration = new RoleConfiguration();
            IdentityUserRoleConfiguration = new IdentityUserRoleConfiguration();
            IdentityUserLoginConfiguration = new IdentityUserLoginConfiguration();
            IdentityRoleClaimConfiguration = new IdentityRoleClaimConfiguration();
            IdentityUserClaimConfiguration = new IdentityUserClaimConfiguration();
            IdentityUserTokenConfiguration = new IdentityUserTokenConfiguration();
            CountryConfiguration = new CountryConfiguration();
            CurrencyConfiguration = new CurrencyConfiguration();
            MakerConfiguration = new MakerConfiguration();
            GoodCategoryConfiguration = new GoodCategoryConfiguration();
            CityConfiguration = new CityConfiguration();
            PosConfiguration = new PosConfiguration();
            PosOperationConfiguration = new PosOperationConfiguration();
            GoodConfiguration = new GoodConfiguration();
            LabeledGoodConfiguration = new LabeledGoodConfiguration();
            GoodImageConfiguration = new GoodImageConfiguration();
            PosImageConfiguration = new PosImageConfiguration();
            BankTransactionInfoConfiguration = new BankTransactionInfoConfiguration();
            FeedbackEntityConfiguration = new FeedbackEntityConfiguration();
            UserBonusPointConfiguration = new UserBonusPointConfiguration();
            PaymentCardConfiguration = new PaymentCardConfiguration();
            CheckItemConfiguration = new CheckItemConfiguration();
            UserFirebaseTokenConfiguration = new UserFirebaseTokenConfiguration();
            PromotionLogConfiguration = new PromotionLogConfiguration();
            AllowedPosModeConfiguration = new AllowedPosModeConfiguration();
            RolePermittedAppFeatureConfiguration = new RolePermittedAppFeatureConfiguration();
            UserNotificationConfiguration = new UserNotificationConfiguration();
            PromotionSettingConfiguration = new PromotionSettingConfiguration();
            MediaContentConfiguration = new MediaContentConfiguration();
            MediaContentToPosPlatformConfiguration = new MediaContentToPosPlatformConfiguration();
            PosMediaContentConfiguration = new PosMediaContentConfiguration();
            ReportUploadingInfoConfiguration = new ReportUploadingInfoConfiguration();
            MessengerContactConfiguration = new MessengerContactConfiguration();
            FiscalizationInfoConfiguration = new FiscalizationInfoConfiguration();
            LabeledGoodTrackingRecordConfiguration = new LabeledGoodTrackingRecordConfiguration();
            PosAbnormalSensorMeasurementConfiguration = new PosAbnormalSensorMeasurementConfiguration();
            PosLogConfiguration = new PosLogConfiguration();
            DiscountConfiguration = new DiscountConfiguration();
            DiscountRuleConfiguration = new DiscountRuleConfiguration();
            DiscountRuleValueConfiguration = new DiscountRuleValueConfiguration();
            PosDiscountConfiguration = new PosDiscountConfiguration();
            PosTemperatureConfiguration = new PosTemperatureConfiguration();
            PosDoorsStateConfiguration = new PosDoorsStateConfiguration();
            CheckItemAuditRecordConfiguration = new CheckItemAuditRecordConfiguration();
            ConfigurationKeyConfiguration = new ConfigurationKeyConfiguration();
            ConfigurationValueConfiguration = new ConfigurationValueConfiguration();
            FiscalizationCheckItemConfiguration = new FiscalizationCheckItemConfiguration();
            PosOperationTransactionConfiguration = new PosOperationTransactionConfiguration();
            BankTransactionInfoVersionTwoConfiguration = new BankTransactionInfoVersionTwoConfiguration();
            FiscalizationInfoVersionTwoConfiguration = new FiscalizationInfoVersionTwoConfiguration();
            PosOperationTransactionCheckItemConfiguration = new PosOperationTransactionCheckItemConfiguration();
            PosScreenTemplateConfiguration = new PosScreenTemplateConfiguration();
            AppFeaturesToRoleConfiguration = new AppFeaturesToRoleConfiguration();
            PointsOfSaleToRoleConfiguration = new PointsOfSaleToRoleConfiguration();
            AppFeatureItemConfiguration = new AppFeatureItemConfiguration();
            DocumentGoodsMovingConfiguration = new DocumentGoodsMovingConfiguration();
            DocumentGoodsMovingTableItemConfiguration = new DocumentGoodsMovingTableItemConfiguration();
            DocumentGoodsMovingLabeledGoodConfiguration = new DocumentGoodsMovingLabeledGoodConfiguration();
        }
    }
}