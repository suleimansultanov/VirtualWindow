using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Repositories.UserNotification;
using System;
using System.Data;
using System.Threading.Tasks;

namespace NasladdinPlace.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IGoodImageRepository GoodImages { get; }
        IGoodRepository Goods { get; }
        IMakerRepository Makers { get; }
        IGoodCategoryRepository GoodCategories { get; }
        ILabeledGoodRepository LabeledGoods { get; }
        ICountryRepository Countries { get; }
        ICityRepository Cities { get; }
        ICurrencyRepository Currencies { get; }
        IPosRepository PointsOfSale { get; }
        IPosOperationRepository PosOperations { get; }
        IPosImageRepository PosImages { get; }
        IFeedbackRepository Feedbacks { get; }
        IUserBonusPointRepository UsersBonusPoints { get; }
        IPosRealTimeInfoRepository PosRealTimeInfos { get; }
        ICheckItemRepository CheckItems { get; }
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IPromotionSettingRepository PromotionSettings { get; }
        IPosMediaContentRepository PosMediaContents { get; }
        IMediaContentToPosPlatformRepository MediaContentToPosPlatforms { get; }
        IReportUploadingInfoRepository ReportsUploadingInfo { get; }
        IMessengerContactRepository MessengerContacts { get; }
        IPosTemperatureRepository PosTemperatures { get; }
        IPosDoorsStateRepository PosDoorsStates { get; }
        IFiscalizationInfosRepository FiscalizationInfos { get; }
        IFiscalizationInfoVersionTwoRepository FiscalizationInfosV2 { get; }
        IPosAbnormalSensorMeasurementRepository PosAbnormalSensorMeasurements { get; }
        IUserNotificationRepository UsersNotifications { get; }
        IPosLogsRepository PosLogs { get; }
        IDiscountsRepository Discounts { get; }
        ICheckItemAuditRecordRepository CheckItemAuditRecords { get; }
        IConfigurationKeyRepository ConfigurationKeys { get; }
        IPosOperationTransactionRepository PosOperationTransactions { get; }
        ILabeledGoodTrackingRecordRepository LabeledGoodTrackingRecords { get; }
        IPosScreenTemplateRepository PosScreenTemplates { get; }
        IAppFeatureItemsRepository AppFeatureItems { get; } 
        IDocumentGoodsMovingLabeledGoodRepository DocumentGoodsMovingLabeledGoods { get; }
        IDocumentGoodsMovingRepository DocumentsGoodsMoving { get; }
        IPosOperationTransactionCheckItemsRepository PosOperationTransactionCheckItems { get; }
        IPaymentCardRepository PaymentCardRepository { get; }

        Task<int> CompleteAsync();
        void BeginTransaction();
        void BeginTransaction(IsolationLevel level);
        void RollbackTransaction();
        void CommitTransaction();

        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

        IReferenceRepository References { get; }
    }
}
