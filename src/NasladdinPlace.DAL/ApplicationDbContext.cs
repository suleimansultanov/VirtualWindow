using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Configuration;
using NasladdinPlace.Core.Models.Discounts;
using NasladdinPlace.Core.Models.Fiscalization;
using NasladdinPlace.Core.Models.Goods;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.DAL.Entities;
using NasladdinPlace.DAL.EntityConfigurations.Contracts;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;

namespace NasladdinPlace.DAL
{
    public class ApplicationDbContext
        : IdentityDbContext<
                ApplicationUser, 
                Role, 
                int, 
                IdentityUserClaim<int>, 
                UserRole, 
                IdentityUserLogin<int>, 
                IdentityRoleClaim<int>, 
                IdentityUserToken<int>>,
          IApplicationDbContext
    {
        private readonly IEntityConfigurations _configurations;

        public DbSet<Pos> PointsOfSale { get; set; }
        public DbSet<Good> Goods { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<PosOperation> PosOperations { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Maker> Makers { get; set; }
        public DbSet<GoodCategory> GoodCategories { get; set; }
        public DbSet<LabeledGood> LabeledGoods { get; set; }
        public DbSet<GoodImage> GoodImages { get; set; }
        public DbSet<PosImage> PosImages { get; set; }
        public DbSet<BankTransactionInfo> BankTransactionInfos { get; set; }
        public DbSet<FeedbackEntity> Feedbacks { get; set; }
        public DbSet<UserBonusPoint> UsersBonusPoints { get; set; }
        public DbSet<PaymentCard> PaymentCards { get; set; }
        public DbSet<CheckItem> CheckItems { get; set; }
        public DbSet<UserFirebaseToken> UserFirebaseTokens { get; set; }
        public DbSet<PromotionLog> PromotionLogs { get; set; }
        public DbSet<AllowedPosMode> AllowedPosModes { get; set; }
        public DbSet<RolePermittedAppFeature> RolesPermittedAppFeatures { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<PromotionSetting> PromotionSettings { get; set; }
        public DbSet<MediaContent> MediaContents { get; set; }
        public DbSet<MediaContentToPosPlatform> MediaContentToPosPlatforms { get; set; }
        public DbSet<PosMediaContent> PosMediaContents { get; set; }
        public DbSet<ReportUploadingInfo> ReportsUploadingInfo { get; set; }
        public DbSet<MessengerContact> MessengerContacts { get; set; }
        public DbSet<FiscalizationInfo> FiscalizationInfos { get; set; }
        public DbSet<LabeledGoodTrackingRecord> LabeledGoodsTrackingHistory { get; set; }
        public DbSet<PosAbnormalSensorMeasurement> PosAbnormalSensorMeasurements { get; set; }
        public DbSet<PosLog> PosLogs { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<DiscountRule> DiscountRules { get; set; }
        public DbSet<DiscountRuleValue> DiscountRuleValues { get; set; }
        public DbSet<PosDiscount> PosDiscounts { get; set; }
        public DbSet<PosTemperature> PosTemperatures { get; set; }
        public DbSet<PosDoorsState> PosDoorsStates { get; set; }
        public DbSet<FiscalizationCheckItem> FiscalizationCheckItems { get; set; }
        public DbSet<CheckItemAuditRecord> CheckItemsAuditHistory { get; set; }
        public DbSet<ConfigurationKey> ConfigurationKeys { get; set; }
        public DbSet<ConfigurationValue> ConfigurationValues { get; set; }
        public DbSet<PosOperationTransaction> PosOperationTransactions { get; set; }
        public DbSet<BankTransactionInfoVersionTwo> BankTransactionInfosVersionTwo { get; set; }
        public DbSet<FiscalizationInfoVersionTwo> FiscalizationInfosVersionTwo { get; set; }
        public DbSet<PosOperationTransactionCheckItem> PosOperationTransactionCheckItems { get; set; }
        public DbSet<PosScreenTemplate> PosScreenTemplates { get; set; }
        public DbSet<ProteinsFatsCarbohydratesCalories> ProteinsFatsCarbohydratesCalories { get; set; }
        public DbSet<AppFeatureItem> AppFeatureItems { get; set; }
        public DbSet<AppFeatureItemsToRole> AppFeatureItemsToRoles { get; set; }
        public DbSet<PointsOfSaleToRole> PointsOfSaleToRoles { get; set; }
        public DbSet<DocumentGoodsMovingTableItem> DocumentGoodsMovingTableItems { get; set; }
        public DbSet<DocumentGoodsMoving> DocumentsGoodsMoving { get; set; }
        public DbSet<DocumentGoodsMovingLabeledGood> DocumentGoodsMovingLabeledGoods { get; set; }

        public ApplicationDbContext(DbContextOptions options, IEntityConfigurations configurations)
            : base(options)
        {
            _configurations = configurations;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity(_configurations.ApplicationUserConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.RoleConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.IdentityUserRoleConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.IdentityUserLoginConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.IdentityRoleClaimConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.IdentityUserClaimConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.IdentityUserTokenConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.CurrencyConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.CountryConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.MakerConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.GoodCategoryConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.CityConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PosConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PosOperationConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.GoodConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.LabeledGoodConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.GoodImageConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PosImageConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.BankTransactionInfoConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.FeedbackEntityConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.UserBonusPointConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PaymentCardConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.CheckItemConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.UserFirebaseTokenConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PromotionLogConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.AllowedPosModeConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.RolePermittedAppFeatureConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.UserNotificationConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PromotionSettingConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.MediaContentConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.MediaContentToPosPlatformConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PosMediaContentConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.ReportUploadingInfoConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.MessengerContactConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.FiscalizationInfoConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.LabeledGoodTrackingRecordConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PosAbnormalSensorMeasurementConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PosLogConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.DiscountConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.DiscountRuleConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.DiscountRuleValueConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PosDiscountConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PosTemperatureConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PosDoorsStateConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.CheckItemAuditRecordConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.ConfigurationKeyConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.ConfigurationValueConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.FiscalizationCheckItemConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PosOperationTransactionConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.BankTransactionInfoVersionTwoConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.FiscalizationInfoVersionTwoConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PosScreenTemplateConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PosOperationTransactionCheckItemConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.AppFeaturesToRoleConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.PointsOfSaleToRoleConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.DocumentGoodsMovingTableItemConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.DocumentGoodsMovingConfiguration.ProvideApplyAction());
            builder.Entity(_configurations.DocumentGoodsMovingLabeledGoodConfiguration.ProvideApplyAction());

            DisableOneToManyCascadeDelete(builder);
        }

        private static void DisableOneToManyCascadeDelete(ModelBuilder builder)
        {
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}