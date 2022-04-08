using System.Collections.Concurrent;
using System.Data;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Repositories.UserNotification;
using NasladdinPlace.Core.Services.Authorization.Models;

namespace NasladdinPlace.Core.Tests.Services.Authorization
{
    public class FakeUnitOfWork : IUnitOfWork
    {
        public IGoodImageRepository GoodImages { get; }
        public IGoodRepository Goods { get; }
        public IMakerRepository Makers { get; }
        public IGoodCategoryRepository GoodCategories { get; }
        public ILabeledGoodRepository LabeledGoods { get; }
        public ICountryRepository Countries { get; }
        public ICityRepository Cities { get; }
        public ICurrencyRepository Currencies { get; }
        public IPosRepository PointsOfSale { get; }
        public IPosOperationRepository PosOperations { get; }
        public IPosImageRepository PosImages { get; }
        public IFeedbackRepository Feedbacks { get; }
        public IUserBonusPointRepository UsersBonusPoints { get; }
        public IPosRealTimeInfoRepository PosRealTimeInfos { get; }
        public ICheckItemRepository CheckItems { get; }
        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public IReportUploadingInfoRepository ReportsUploadingInfo { get; }
        public IMessengerContactRepository MessengerContacts { get; }
        public IReferenceRepository References { get; }
        public IPromotionSettingRepository PromotionSettings { get; }
        public IPosMediaContentRepository PosMediaContents { get; }
        public IMediaContentToPosPlatformRepository MediaContentToPosPlatforms { get; }
        public IPosTemperatureRepository PosTemperatures { get; }
        public IPosDoorsStateRepository PosDoorsStates { get; }
        public IFiscalizationInfosRepository FiscalizationInfos { get; }
        public IFiscalizationInfoVersionTwoRepository FiscalizationInfosV2 { get; }
        public IPosAbnormalSensorMeasurementRepository PosAbnormalSensorMeasurements { get; }
        public IUserNotificationRepository UsersNotifications { get; }
        public IPosLogsRepository PosLogs { get; }
        public ICheckItemAuditRecordRepository CheckItemAuditRecords { get; }
        public IConfigurationKeyRepository ConfigurationKeys { get; }
        public IPosOperationTransactionRepository PosOperationTransactions { get; }
        public ILabeledGoodTrackingRecordRepository LabeledGoodTrackingRecords { get; }
        public IDiscountsRepository Discounts { get; }
        public IPosScreenTemplateRepository PosScreenTemplates { get; }
        public IAppFeatureItemsRepository AppFeatureItems { get; }
        public IDocumentGoodsMovingLabeledGoodRepository DocumentGoodsMovingLabeledGoods { get; }
        public IDocumentGoodsMovingRepository DocumentsGoodsMoving { get; }
        public IPosOperationTransactionCheckItemsRepository PosOperationTransactionCheckItems { get; }
        public IPaymentCardRepository PaymentCardRepository { get; }

        public FakeUnitOfWork(ConcurrentDictionary<AccessGroup, Role> roleByAccessGroupDictionary)
        {
            Roles = new FakeRoleRepository(roleByAccessGroupDictionary);
        }
        
        public Task<int> CompleteAsync()
        {
            return Task.FromResult(1);
        }

        public void BeginTransaction()
        {
            throw new System.NotImplementedException();
        }

        public void BeginTransaction(IsolationLevel level)
        {
            throw new System.NotImplementedException();
        }

        public void RollbackTransaction()
        {
            throw new System.NotImplementedException();
        }

        public void CommitTransaction()
        {
            throw new System.NotImplementedException();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            // do nothing
        }
    }
}