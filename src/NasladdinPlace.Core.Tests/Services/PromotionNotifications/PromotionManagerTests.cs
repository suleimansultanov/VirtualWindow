using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.Core.Services.Payment.Card;
using NasladdinPlace.Core.Services.PromotionNotifications.PromotionManager;
using NasladdinPlace.Payment.Models;
using Xunit;

namespace NasladdinPlace.Core.Tests.Services.PromotionNotifications
{
    public class PromotionManagerTests
    {
        private readonly Mock<IRepository<ApplicationUser>> _mockIUserRepository;
        private readonly Mock<IRepository<PromotionLog>> _mockIPromotionLogRepository;
        private readonly Mock<IPromotionSettingRepository> _mockIPromotionSettingRepository;
        private readonly PromotionManager _promotionManager;

        private const int DefaultUserId = 1;
        private const string DefaultCardToken = "Default Card Token";

        public PromotionManagerTests()
        {
            _mockIUserRepository = new Mock<IRepository<ApplicationUser>>();
            _mockIPromotionLogRepository = new Mock<IRepository<PromotionLog>>();
            _mockIPromotionSettingRepository = new Mock<IPromotionSettingRepository>();

            var mockUoW = new Mock<IUnitOfWork>();

            mockUoW.Setup(u => u.GetRepository<ApplicationUser>()).Returns(_mockIUserRepository.Object);
            mockUoW.Setup(u => u.GetRepository<PromotionLog>()).Returns(_mockIPromotionLogRepository.Object);
            mockUoW.Setup(u => u.PromotionSettings).Returns(_mockIPromotionSettingRepository.Object);
            var mockUoWFactory = new Mock<IUnitOfWorkFactory>();
            mockUoWFactory.Setup(f => f.MakeUnitOfWork()).Returns(mockUoW.Object);

            _promotionManager = new PromotionManager(mockUoWFactory.Object);            
        }

        [Fact]
        public async Task GetVerifyPhoneNumberPromotionNotifications_OneUsersWithNotConfirmedPhoneNumbers_ShouldReturnOneNotification()
        {
            InitPromotionSettings(isEnabled: true, isNotificationEnabled: true);

            var usersInDb = new TestAsyncEnumerable<ApplicationUser>(
                                new List<ApplicationUser>
                                    {
                                        new ApplicationUser()
                                    }
                                ).AsQueryable();

            var promotionLogs = new TestAsyncEnumerable<PromotionLog>(new List<PromotionLog>()).AsQueryable();

            _mockIUserRepository.Setup(r => r.GetAll()).Returns(usersInDb.AsQueryable());
            _mockIPromotionLogRepository.Setup(r => r.GetAll()).Returns(promotionLogs);

            var promotionNotifications = await _promotionManager.GetPromotionNotificationsByType(PromotionType.VerifyPhoneNumber);

            promotionNotifications.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetVerifyPhoneNumberPromotionNotifications_OneUsersWithNotConfirmedPhoneNumbersWithExistiongPromotionLog_ShouldReturnEmptyNotificationsList()
        {
            InitPromotionSettings(isEnabled: true, isNotificationEnabled: true);

            var usersInDb = new TestAsyncEnumerable<ApplicationUser>(
                new List<ApplicationUser>
                {
                    new ApplicationUser
                    {
                        Id = DefaultUserId
                    }
                }).AsQueryable();

            var promotionLogs = new TestAsyncEnumerable<PromotionLog>(new List<PromotionLog>
            {
                new PromotionLog(DefaultUserId, PromotionType.VerifyPhoneNumber, NotificationType.Sms)
            }).AsQueryable();

            _mockIUserRepository.Setup(r => r.GetAll()).Returns(usersInDb.AsQueryable());
            _mockIPromotionLogRepository.Setup(r => r.GetAll()).Returns(promotionLogs);

            var promotionNotifications = await _promotionManager.GetPromotionNotificationsByType(PromotionType.VerifyPhoneNumber);

            promotionNotifications.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetVerifyPaymentCardPromotionNotifications_OneUsersWithNotConfirmedPaymentCardAndConfirmedPhoneNumber_ShouldReturnOneNotification()
        {
            InitPromotionSettings(isEnabled: true, isNotificationEnabled: true);

            var usersInDb = new TestAsyncEnumerable<ApplicationUser>(
                                new List<ApplicationUser>
                                    {
                                        new ApplicationUser
                                        {
                                            PhoneNumberConfirmed = true
                                        }
                                    }
                                ).AsQueryable();

            var promotionLogs = new TestAsyncEnumerable<PromotionLog>(new List<PromotionLog>()).AsQueryable();

            _mockIUserRepository.Setup(r => r.GetAll()).Returns(usersInDb.AsQueryable());
            _mockIPromotionLogRepository.Setup(r => r.GetAll()).Returns(promotionLogs);

            var promotionNotifications = await _promotionManager.GetPromotionNotificationsByType(PromotionType.VerifyPaymentCard);

            promotionNotifications.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetVerifyPaymentCardPromotionNotifications_OneUsersWithNotConfirmedPaymentCardAndConfirmedPhoneNumberWithExistingPromotionLog_ShouldReturnEmptyNotificationsList()
        {
            InitPromotionSettings(isEnabled: true, isNotificationEnabled: true);

            var usersInDb = new TestAsyncEnumerable<ApplicationUser>(
                new List<ApplicationUser>
                {
                    new ApplicationUser
                    {
                        Id = DefaultUserId,
                        PhoneNumberConfirmed = true                        
                    }
                }).AsQueryable();

            var promotionLogs = new TestAsyncEnumerable<PromotionLog>(new List<PromotionLog>
            {
                new PromotionLog(DefaultUserId, PromotionType.VerifyPaymentCard, NotificationType.Sms)
            }).AsQueryable();

            _mockIUserRepository.Setup(r => r.GetAll()).Returns(usersInDb.AsQueryable());
            _mockIPromotionLogRepository.Setup(r => r.GetAll()).Returns(promotionLogs);

            var promotionNotifications = await _promotionManager.GetPromotionNotificationsByType(PromotionType.VerifyPaymentCard);

            promotionNotifications.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetFirstPayPromotionNotifications_OneUsersWithConfirmedPaymentCardAndConfirmedPhoneNumber_ShouldReturnOneNotification()
        {
            InitPromotionSettings(isEnabled: true, isNotificationEnabled: true);

            var usersInDb = ProvideUsersInDatabaseWithActivePaymentCards();

            var promotionLogs = new TestAsyncEnumerable<PromotionLog>(new List<PromotionLog>()).AsQueryable();

            _mockIUserRepository.Setup(r => r.GetAll()).Returns(usersInDb.AsQueryable());
            _mockIPromotionLogRepository.Setup(r => r.GetAll()).Returns(promotionLogs);

            var promotionNotifications = await _promotionManager.GetPromotionNotificationsByType(PromotionType.FirstPay);

            promotionNotifications.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetFirstPayPromotionNotifications_OneUsersWithConfirmedPaymentCardAndConfirmedPhoneNumberWithExistingPromotionLog_ShouldReturnEmptyNotificationsList()
        {
            InitPromotionSettings(isEnabled: true, isNotificationEnabled: true);

            var usersInDb = ProvideUsersInDatabaseWithActivePaymentCards();

            var promotionLogs = new TestAsyncEnumerable<PromotionLog>(new List<PromotionLog>
            {
                new PromotionLog(DefaultUserId, PromotionType.FirstPay, NotificationType.Sms)
            }).AsQueryable();

            _mockIUserRepository.Setup(r => r.GetAll()).Returns(usersInDb.AsQueryable());
            _mockIPromotionLogRepository.Setup(r => r.GetAll()).Returns(promotionLogs);

            var promotionNotifications = await _promotionManager.GetPromotionNotificationsByType(PromotionType.FirstPay);

            promotionNotifications.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetFirstPayPromotionNotifications_OneUsersWithConfirmedPaymentCardAndConfirmedPhoneNumberWithPaymentsOperations_ShouldReturnEmptyNotificationsList()
        {
            InitPromotionSettings(isEnabled: true, isNotificationEnabled: true);

            var usersInDb = ProvideUsersInDatabaseWithActivePaymentCards();
            var posOperation = new PosOperation(
                id: default(int),
                userId: DefaultUserId,
                posId: default(int),
                dateStarted: default(DateTime),
                datePaid: default(DateTime),
                completionInitiationDate: default(DateTime),
                dateCompleted: default(DateTime)
            );
            usersInDb.ForEach(u => u.PosOperations.Add(posOperation));

            var promotionLogs = new TestAsyncEnumerable<PromotionLog>(new List<PromotionLog>()).AsQueryable();

            _mockIUserRepository.Setup(r => r.GetAll()).Returns(usersInDb.AsQueryable());
            _mockIPromotionLogRepository.Setup(r => r.GetAll()).Returns(promotionLogs);

            var promotionNotifications = await _promotionManager.GetPromotionNotificationsByType(PromotionType.FirstPay);

            promotionNotifications.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetVerifyPhoneNumberPromotionNotifications_OneUsersWithNotConfirmedPhoneNumbersPromotionDisabled_ShouldReturnEmptyNotificationsList()
        {
            InitPromotionSettings(isEnabled: false, isNotificationEnabled: false);

            var usersInDb = new TestAsyncEnumerable<ApplicationUser>(
                new List<ApplicationUser>
                {
                    new ApplicationUser()
                }
            ).AsQueryable();

            var promotionLogs = new TestAsyncEnumerable<PromotionLog>(new List<PromotionLog>()).AsQueryable();

            _mockIUserRepository.Setup(r => r.GetAll()).Returns(usersInDb.AsQueryable());
            _mockIPromotionLogRepository.Setup(r => r.GetAll()).Returns(promotionLogs);

            var promotionNotifications = await _promotionManager.GetPromotionNotificationsByType(PromotionType.VerifyPhoneNumber);

            promotionNotifications.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetVerifyPaymentCardPromotionNotifications_OneUsersWithNotConfirmedPaymentCardAndConfirmedPhoneNumberAndPromotionDisabled_ShouldReturnEmptyNotificationsList()
        {
            InitPromotionSettings(isEnabled: false, isNotificationEnabled: false);

            var usersInDb = new TestAsyncEnumerable<ApplicationUser>(
                new List<ApplicationUser>
                {
                    new ApplicationUser
                    {
                        PhoneNumberConfirmed = true
                    }
                }
            ).AsQueryable();

            var promotionLogs = new TestAsyncEnumerable<PromotionLog>(new List<PromotionLog>()).AsQueryable();

            _mockIUserRepository.Setup(r => r.GetAll()).Returns(usersInDb.AsQueryable());
            _mockIPromotionLogRepository.Setup(r => r.GetAll()).Returns(promotionLogs);

            var promotionNotifications = await _promotionManager.GetPromotionNotificationsByType(PromotionType.VerifyPaymentCard);

            promotionNotifications.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetFirstPayPromotionNotifications_OneUsersWithConfirmedPaymentCardAndConfirmedPhoneNumberAndPromotionDisabled_ShouldReturnEmptyNotificationsList()
        {
            InitPromotionSettings(isEnabled: false, isNotificationEnabled: false);

            var usersInDb = ProvideUsersInDatabaseWithActivePaymentCards();

            var promotionLogs = new TestAsyncEnumerable<PromotionLog>(new List<PromotionLog>()).AsQueryable();

            _mockIUserRepository.Setup(r => r.GetAll()).Returns(usersInDb.AsQueryable());
            _mockIPromotionLogRepository.Setup(r => r.GetAll()).Returns(promotionLogs);

            var promotionNotifications = await _promotionManager.GetPromotionNotificationsByType(PromotionType.FirstPay);

            promotionNotifications.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetVerifyPhoneNumberPromotionNotifications_OneUsersWithNotConfirmedPhoneNumbersPromotionEnabledNotificationDisabled_ShouldReturnEmptyNotificationsList()
        {
            InitPromotionSettings(isEnabled: true, isNotificationEnabled: false);

            var usersInDb = new TestAsyncEnumerable<ApplicationUser>(
                new List<ApplicationUser>
                {
                    new ApplicationUser()
                }
            ).AsQueryable();

            var promotionLogs = new TestAsyncEnumerable<PromotionLog>(new List<PromotionLog>()).AsQueryable();

            _mockIUserRepository.Setup(r => r.GetAll()).Returns(usersInDb.AsQueryable());
            _mockIPromotionLogRepository.Setup(r => r.GetAll()).Returns(promotionLogs);

            var promotionNotifications = await _promotionManager.GetPromotionNotificationsByType(PromotionType.VerifyPhoneNumber);

            promotionNotifications.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetVerifyPaymentCardPromotionNotifications_OneUsersWithNotConfirmedPaymentCardAndConfirmedPhoneNumberAndPromotionEnabledNotificationDisabled_ShouldReturnEmptyNotificationsList()
        {
            InitPromotionSettings(isEnabled: true, isNotificationEnabled: false);

            var usersInDb = new TestAsyncEnumerable<ApplicationUser>(
                new List<ApplicationUser>
                {
                    new ApplicationUser
                    {
                        PhoneNumberConfirmed = true
                    }
                }
            ).AsQueryable();

            var promotionLogs = new TestAsyncEnumerable<PromotionLog>(new List<PromotionLog>()).AsQueryable();

            _mockIUserRepository.Setup(r => r.GetAll()).Returns(usersInDb.AsQueryable());
            _mockIPromotionLogRepository.Setup(r => r.GetAll()).Returns(promotionLogs);

            var promotionNotifications = await _promotionManager.GetPromotionNotificationsByType(PromotionType.VerifyPaymentCard);

            promotionNotifications.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetFirstPayPromotionNotifications_OneUsersWithConfirmedPaymentCardAndConfirmedPhoneNumberAndPromotionEnabledNotificationDisabled_ShouldReturnEmptyNotificationsList()
        {
            InitPromotionSettings(isEnabled: true, isNotificationEnabled: false);

            var usersInDb = ProvideUsersInDatabaseWithActivePaymentCards();

            var promotionLogs = new TestAsyncEnumerable<PromotionLog>(new List<PromotionLog>()).AsQueryable();

            _mockIUserRepository.Setup(r => r.GetAll()).Returns(usersInDb.AsQueryable());
            _mockIPromotionLogRepository.Setup(r => r.GetAll()).Returns(promotionLogs);

            var promotionNotifications = await _promotionManager.GetPromotionNotificationsByType(PromotionType.FirstPay);

            promotionNotifications.Should().HaveCount(0);
        }

        private void InitPromotionSettings(bool isEnabled, bool isNotificationEnabled)
        {
            _mockIPromotionSettingRepository.Setup(r => r.GetByPromotionTypeAsync(PromotionType.VerifyPhoneNumber))
                .Returns(Task.FromResult(new PromotionSetting(PromotionType.VerifyPhoneNumber, 50M, isEnabled,
                    isNotificationEnabled, new TimeSpan(10, 30, 00))
                ));

            _mockIPromotionSettingRepository.Setup(r => r.GetByPromotionTypeAsync(PromotionType.VerifyPaymentCard))
                .Returns(Task.FromResult(new PromotionSetting(PromotionType.VerifyPaymentCard, 0M, isEnabled,
                    isNotificationEnabled, new TimeSpan(10, 35, 00))
                ));

            _mockIPromotionSettingRepository.Setup(r => r.GetByPromotionTypeAsync(PromotionType.FirstPay))
                .Returns(Task.FromResult(new PromotionSetting(PromotionType.FirstPay, 50M, isEnabled,
                    isNotificationEnabled, new TimeSpan(11, 00, 00))
                ));
        }

        private static TestAsyncEnumerable<ApplicationUser> ProvideUsersInDatabaseWithActivePaymentCards()
        {
            var user = new ApplicationUser
            {
                Id = DefaultUserId,
                PhoneNumberConfirmed = true
            };
            var paymentCardNumber = new PaymentCardNumber("123456", "7890");
            var extendedPaymentCardInfo = 
                new ExtendedPaymentCardInfo(paymentCardNumber, DateTime.UtcNow, DefaultCardToken);
            user.SetActivePaymentCard(extendedPaymentCardInfo);
            user.SetActivePaymentCard(0);

            var users = new List<ApplicationUser>
            {
                user
            };
            
            return new TestAsyncEnumerable<ApplicationUser>(users);
        }
    }
}
