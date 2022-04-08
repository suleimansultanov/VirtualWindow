using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Repositories.UserNotification;
using NasladdinPlace.DAL.DataStore.PosRealTimeInfo;
using NasladdinPlace.DAL.Repositories;

namespace NasladdinPlace.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly ConcurrentDictionary<Type, object> _repositories;

        private IDbContextTransaction _transaction;

        private bool _disposed;

        public IGoodImageRepository GoodImages { get; }
        public IGoodRepository Goods { get; }
        public IMakerRepository Makers { get; }
        public IGoodCategoryRepository GoodCategories { get; }
        public ILabeledGoodRepository LabeledGoods { get; }
        public ICountryRepository Countries { get; }
        public ICityRepository Cities { get; }
        public IPosRepository PointsOfSale { get; }
        public IPosOperationRepository PosOperations { get; }
        public IPosImageRepository PosImages { get; }
        public ICurrencyRepository Currencies { get; }
        public IFeedbackRepository Feedbacks { get; }
        public IUserBonusPointRepository UsersBonusPoints { get; }
        public IPosRealTimeInfoRepository PosRealTimeInfos { get; }
        public IUserRepository Users { get; }
        public IRoleRepository Roles { get; }
        public ICheckItemRepository CheckItems { get; }
        public IReferenceRepository References { get; }
        public IPromotionSettingRepository PromotionSettings { get; }
        public IPosMediaContentRepository PosMediaContents { get; }
        public IMediaContentToPosPlatformRepository MediaContentToPosPlatforms { get; }
        public IReportUploadingInfoRepository ReportsUploadingInfo { get; }
        public IMessengerContactRepository MessengerContacts { get; }
        public IPosTemperatureRepository PosTemperatures { get; }
        public IPosDoorsStateRepository PosDoorsStates { get; }
        public IFiscalizationInfosRepository FiscalizationInfos { get; }
        public IPosAbnormalSensorMeasurementRepository PosAbnormalSensorMeasurements { get; set; }
        public IUserNotificationRepository UsersNotifications { get; }
        public IPosLogsRepository PosLogs { get; }
        public ICheckItemAuditRecordRepository CheckItemAuditRecords { get; }
        public IConfigurationKeyRepository ConfigurationKeys { get; }
        public ILabeledGoodTrackingRecordRepository LabeledGoodTrackingRecords { get; }
        public IDiscountsRepository Discounts { get; }
        public IFiscalizationInfoVersionTwoRepository FiscalizationInfosV2 { get; }
        public IPosOperationTransactionRepository PosOperationTransactions { get; }
        public IPosScreenTemplateRepository PosScreenTemplates { get; }
        public IAppFeatureItemsRepository AppFeatureItems { get; }
        public IDocumentGoodsMovingLabeledGoodRepository DocumentGoodsMovingLabeledGoods { get; }
        public IDocumentGoodsMovingRepository DocumentsGoodsMoving { get; }
        public IPosOperationTransactionCheckItemsRepository PosOperationTransactionCheckItems { get; }
        public IPaymentCardRepository PaymentCardRepository { get; }

        public UnitOfWork(ApplicationDbContext context, IPosRealTimeInfoDataStore posRealTimeInfoDataStore)
        {
            _context = context;
            _repositories = new ConcurrentDictionary<Type, object>();

            GoodImages = new GoodImageRepository(context);
            Goods = new GoodRepository(context);
            Makers = new MakerRepository(context);
            GoodCategories = new GoodCategoryRepository(context);
            LabeledGoods = new LabeledGoodRepository(context);
            Countries = new CountryRepository(context);
            Cities = new CityRepository(context);
            PointsOfSale = new PosRepository(context);
            PosOperations = new PosOperationRepository(context);
            PosImages = new PosImageRepository(context);
            Currencies = new CurrencyRepository(context);
            Feedbacks = new FeedbackRepository(context);
            UsersBonusPoints = new UserBonusPointRepository(context);
            PosRealTimeInfos = new PosRealTimeInfoRepository(context, posRealTimeInfoDataStore);
            Users = new UserRepository(context);
            CheckItems = new CheckItemRepository(context);
            Roles = new RoleRepository(context);
            References = new ReferenceRepository(context);
            PromotionSettings = new PromotionSettingRepository(context);
            PosMediaContents = new PosMediaContentRepository(context);
            MediaContentToPosPlatforms = new MediaContentToPosPlatformRepository(context);
            ReportsUploadingInfo = new ReportUploadingInfoRepository(context);
            PosTemperatures = new PosTemperatureRepository(context);
            PosDoorsStates = new PosDoorsStateRepository(context);
            FiscalizationInfos = new FiscalizationInfosRepository(context);
            PosAbnormalSensorMeasurements = new PosAbnormalSensorMeasurementsRepository(context);
            UsersNotifications = new UserNotificationRepository(context);
            PosLogs = new PosLogsRepository(context);
            CheckItemAuditRecords = new CheckItemAuditRecordRepository(context);
            Discounts = new DiscountsRepository(context);
            ConfigurationKeys = new ConfigurationKeyRepository(context);
            FiscalizationInfosV2 = new FiscalizationInfoVersionTwoRepository(context);
            PosOperationTransactions = new PosOperationTransactionRepository(context);
            LabeledGoodTrackingRecords = new LabeledGoodTrackingRecordRepository(context);
            PosScreenTemplates = new PosScreenTemplateRepository(context);
            AppFeatureItems = new AppFeatureItemsRepository(context);
            DocumentsGoodsMoving = new DocumentGoodsMovingRepository(context);
            DocumentGoodsMovingLabeledGoods = new DocumentGoodsMovingLabeledGoodRepository(context);
            MessengerContacts = new MessengerContactRepository(context);
            PosOperationTransactionCheckItems = new PosOperationTransactionCheckItemsRepository(context);
            PaymentCardRepository = new PaymentCardRepository(context);
        }

        public Task<int> CompleteAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void BeginTransaction(IsolationLevel level)
        {
            _transaction = _context.Database.BeginTransaction(level);
        }

        public void CommitTransaction()
        {
            if (_transaction == null) return;
            
            _transaction.Commit();
            _transaction.Dispose();

            _transaction = null;
        }

        public void RollbackTransaction()
        {
            if (_transaction == null) return;
            
            _transaction.Rollback();
            _transaction.Dispose();

            _transaction = null;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return _repositories.GetOrAdd(typeof(TEntity), new Repository<TEntity>(_context)) as IRepository<TEntity>;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _context.Dispose();

            _disposed = true;
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}

