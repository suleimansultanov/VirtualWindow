using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Tests.Constants;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Application.Dtos.Feedback;
using NasladdinPlace.Application.Services.Feedbacks.Contracts;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Feedback;
using NasladdinPlace.Core.Services.Feedback.Builder.SenderInfo;
using NasladdinPlace.Core.Services.Feedback.Printer;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.DAL.Contracts;
using NasladdinPlace.DAL.Entities;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.Utilities.DateTimeConverter;
using NUnit.Framework;

namespace NasladdinPlace.Api.Tests.Scenarios.Feedbacks
{
    public class FeedbackCreationScenario : TestsBase
    {
        private const int AddedFeedbackEntityId = 1;
        private const int DefaultUserId = 1;
        private const int DefaultPosId = 2;
        private const int DefaultFirstLabeledGoodId = 1;
        private const int DefaultSecondLabeledGoodId = 2;
        private const int DefaultFirstLPosOperationId = 1;
        private const int DefaultSecondPosOperationId = 2;
        private const decimal DefaultFirstPrice = 10M;
        private const decimal DefaultSecondPrice = 15M;

        private ISenderInfoFactory _senderInfoFactory;
        private IFeedbackPrinter _feedbackPrinter;

        private IFeedbackAppService _feedbackAppService;

        private IApplicationDbContextFactory _dbContextFactory;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            
            Seeder.Seed(new CountriesDataSet());
            Seeder.Seed(new CitiesDataSet());
            Seeder.Seed(new CurrenciesDataSet());
            Seeder.Seed(new MakersDataSet());
            Seeder.Seed(new GoodsDataSet());
            Seeder.Seed(new UsersDataSet());
            Seeder.Seed(new PointsOfSaleDataSet());
            Seeder.Seed(LabeledGoodsDataSet.FromPosId(DefaultPosId));
            Seeder.Seed(new PaymentCardsDataSet(DefaultUserId));

            Mapper.Reset();
            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            var serviceProvider = TestServiceProviderFactory.Create();
            _senderInfoFactory = serviceProvider.GetRequiredService<ISenderInfoFactory>();
            _feedbackPrinter = serviceProvider.GetRequiredService<IFeedbackPrinter>();
            _dbContextFactory = serviceProvider.GetRequiredService<IApplicationDbContextFactory>();

            _feedbackAppService = serviceProvider.GetRequiredService<IFeedbackAppService>();
        }

        [Test]
        public void AddFeedback_UserUnauthorized_ShouldReturnExpectedFeedbackMessageContent()
        {
            var feedback = GetFixtureFeedbackForUnauthorizedUser();
            var message = _feedbackPrinter.Print(feedback);

            EnsureFeedbackMessageForUnauthorizedUserHasExpectedContent(feedback, message);
        }

        [Test]
        public void AddFeedback_UserUnauthorized_ShouldAddExpectedFeedbackToDb()
        {
            _feedbackAppService.CreateFeedbackAsync(CreateFixtureFeedbackDto(), null).GetAwaiter().GetResult();

            var expectedFeedbackEntity = new ExpectedFeedbackEntity(
                appVersion: FeedbackRussianMessageContents.AppVersion,
                content: FeedbackRussianMessageContents.FeedbackContent,
                deviceName: FeedbackRussianMessageContents.DeviceName,
                deviceOperatingSystem: FeedbackRussianMessageContents.DeviceOperatingSystem,
                phoneNumber: FeedbackRussianMessageContents.UserPhoneNumber,
                dateCreated: DateTime.UtcNow.Date
                );
            EnsureExpectedFeedbackEntityCreated(expectedFeedbackEntity);
        }

        [Test]
        public void AddFeedback_UserAuthorizedAndPosOperationWithCheckItemsExist_ShouldReturnExpectedFeedbackMessageContent()
        {
            var user = GetFixtureUser();

            SeedPaidPosOperationsWithCheckItems();

            var feedback = GetFixtureFeedback(user);

            var message = _feedbackPrinter.Print(feedback);

            EnsureFeedbackMessageForPosOperationWithChecksHasExpectedContent(feedback, message);
        }

        [Test]
        public void AddFeedback_UserAuthorizedAndPosOperationWithCheckItemsUnpaid_ShouldReturnExpectedFeedbackMessageContent()
        {
            var user = GetFixtureUser();

            SeedCompletedPosOperationsWithUnpaidCheckItems();

            var feedback = GetFixtureFeedback(user);

            var message = _feedbackPrinter.Print(feedback);

            EnsureFeedbackMessageForPosOperationWithUnpaidChecksHasExpectedContent(feedback, message);
        }

        [Test]
        public void AddFeedback_UserAuthorizedAndPosOperationWithCheckItemsExist_ShouldAddExpectedFeedbackToDb()
        {
            var user = GetFixtureUser();

            SeedPaidPosOperationsWithCheckItems();
            
            _feedbackAppService.CreateFeedbackAsync(CreateFixtureFeedbackDto(), user).GetAwaiter().GetResult();

            var expectedFeedbackEntity = new ExpectedFeedbackEntity(
                appVersion: FeedbackRussianMessageContents.AppVersion,
                content: FeedbackRussianMessageContents.FeedbackContent,
                deviceName: FeedbackRussianMessageContents.DeviceName,
                deviceOperatingSystem: FeedbackRussianMessageContents.DeviceOperatingSystem,
                phoneNumber: FeedbackRussianMessageContents.UserPhoneNumber,
                dateCreated: DateTime.UtcNow.Date
            )
            {
                UserId = DefaultUserId,
                PosId = DefaultPosId
            };
            EnsureExpectedFeedbackEntityCreated(expectedFeedbackEntity);
        }

        [Test]
        public void AddFeedback_UserAuthorizedAndPosOperationsCheckItemsEmpty_ShouldReturnExpectedFeedbackMessageContent()
        {
            var user = GetFixtureUser();

            SeedPosOperationsWithNoChecks();

            var feedback = GetFixtureFeedback(user);

            var message = _feedbackPrinter.Print(feedback);

            EnsureFeedbackMessageForNoLastPurchaseHasExpectedContent(feedback, message);
        }

        [Test]
        public void AddFeedback_UserAuthorizedAndPosOperationsCheckItemsEmpty_ShouldAddExpectedFeedbackToDb()
        {
            var user = GetFixtureUser();

            SeedPosOperationsWithNoChecks();

            _feedbackAppService.CreateFeedbackAsync(CreateFixtureFeedbackDto(), user).GetAwaiter().GetResult();

            var expectedFeedbackEntity = new ExpectedFeedbackEntity(
                appVersion: FeedbackRussianMessageContents.AppVersion,
                content: FeedbackRussianMessageContents.FeedbackContent,
                deviceName: FeedbackRussianMessageContents.DeviceName,
                deviceOperatingSystem: FeedbackRussianMessageContents.DeviceOperatingSystem,
                phoneNumber: FeedbackRussianMessageContents.UserPhoneNumber,
                dateCreated: DateTime.UtcNow.Date
            )
            {
                UserId = DefaultUserId,
                PosId = DefaultPosId
            };
            EnsureExpectedFeedbackEntityCreated(expectedFeedbackEntity);
        }

        [Test]
        public void AddFeedback_UserAuthorizedAndPosOperationsCheckItemsPending_ShouldReturnExpectedFeedbackMessageContent()
        {
            var user = GetFixtureUser();

            SeedOpenedPosOperations();

            var feedback = GetFixtureFeedback(user);
            var message = _feedbackPrinter.Print(feedback);

            EnsureFeedbackMessageForNoLastPurchaseHasExpectedContent(feedback, message);
        }

        [Test]
        public void AddFeedback_UserAuthorizedAndPosOperationsCheckItemsPending_ShouldAddExpectedFeedbackToDb()
        {
            var user = GetFixtureUser();

            SeedOpenedPosOperations();

            _feedbackAppService.CreateFeedbackAsync(CreateFixtureFeedbackDto(), user).GetAwaiter().GetResult();

            var expectedFeedbackEntity = new ExpectedFeedbackEntity(
                appVersion: FeedbackRussianMessageContents.AppVersion,
                content: FeedbackRussianMessageContents.FeedbackContent,
                deviceName: FeedbackRussianMessageContents.DeviceName,
                deviceOperatingSystem: FeedbackRussianMessageContents.DeviceOperatingSystem,
                phoneNumber: FeedbackRussianMessageContents.UserPhoneNumber,
                dateCreated: DateTime.UtcNow.Date
            )
            {
                UserId = DefaultUserId,
                PosId = DefaultPosId
            };
            EnsureExpectedFeedbackEntityCreated(expectedFeedbackEntity);
        }

        [Test]
        public void AddFeedback_UserAuthorizedAndPosOperationsNotExist_ShouldReturnExpectedFeedbackMessageContent()
        {
            var user = GetFixtureUser();
            var feedback = GetFixtureFeedback(user);
            var message = _feedbackPrinter.Print(feedback);

            EnsureFeedbackMessageForNotExistingPosOperationHasExpectedContent(feedback, message);
        }

        [Test]
        public void AddFeedback_UserAuthorizedAndPosOperationsNotExist_ShouldAddExpectedFeedbackToDb()
        {
            var user = GetFixtureUser();
            
            _feedbackAppService.CreateFeedbackAsync(CreateFixtureFeedbackDto(), user).GetAwaiter().GetResult();

            var expectedFeedbackEntity = new ExpectedFeedbackEntity(
                appVersion: FeedbackRussianMessageContents.AppVersion,
                content: FeedbackRussianMessageContents.FeedbackContent,
                deviceName: FeedbackRussianMessageContents.DeviceName,
                deviceOperatingSystem: FeedbackRussianMessageContents.DeviceOperatingSystem,
                phoneNumber: FeedbackRussianMessageContents.UserPhoneNumber,
                dateCreated: DateTime.UtcNow.Date
            )
            {
                UserId = DefaultUserId
            };
            EnsureExpectedFeedbackEntityCreated(expectedFeedbackEntity);
        }

        private static void EnsureFeedbackMessageForPosOperationWithChecksHasExpectedContent(Feedback feedback, string message)
        {
            var expectedMessage =
                GetAuthorizedUserFeedbackHeaderTemplate() +
                GetPosInfoTemplate() + 
                GetDateCreatedTemplate(feedback) +
                GetCheckInfoTemplate() +
                GetPurchaseDateTemplate(feedback) +
                GetUserBalanceTemplate(decimal.Zero) +
                GetDeviceInfoTemplate();

            var expectedUnescapedMessage = UnescapeMessage(expectedMessage);
            var unescapedMessage = UnescapeMessage(message);
            unescapedMessage.Should().Be(expectedUnescapedMessage);
        }

        private static void EnsureFeedbackMessageForPosOperationWithUnpaidChecksHasExpectedContent(Feedback feedback, string message)
        {
            var expectedMessage =
                GetAuthorizedUserFeedbackHeaderTemplate() +
                GetPosInfoTemplate() + 
                GetDateCreatedTemplate(feedback) +
                GetCheckInfoTemplate() +
                GetPurchaseDateTemplate(feedback) +
                GetUserBalanceTemplate(-DefaultSecondPrice) +
                GetDeviceInfoTemplate();

            var expectedUnescapedMessage = UnescapeMessage(expectedMessage);
            var unescapedMessage = UnescapeMessage(message);
            unescapedMessage.Should().Be(expectedUnescapedMessage);
        }

        private static void EnsureFeedbackMessageForNoLastPurchaseHasExpectedContent(Feedback feedback, string message)
        {
            var expectedMessage =
                GetAuthorizedUserFeedbackHeaderTemplate() +
                GetPosInfoTemplate() +
                GetDateCreatedTemplate(feedback) +
                GetNoInfoTemplate(FeedbackRussianMessageContents.FeedbackLastPurchaseEmptyTemplate) +
                GetPurchaseDateTemplate(feedback) +
                GetUserBalanceTemplate(decimal.Zero) +
                GetDeviceInfoTemplate();

            var expectedUnescapedMessage = UnescapeMessage(expectedMessage);
            var unescapedMessage = UnescapeMessage(message);
            unescapedMessage.Should().Be(expectedUnescapedMessage);
        }

        private static void EnsureFeedbackMessageForNotExistingPosOperationHasExpectedContent(Feedback feedback, string message)
        {
            var expectedMessage =
                GetAuthorizedUserFeedbackHeaderTemplate() +
                GetNoInfoTemplate(FeedbackRussianMessageContents.FeedbackPosEmptyTemplate) +
                GetDateCreatedTemplate(feedback) +
                GetNoPurchasesYetTemplate(FeedbackRussianMessageContents.FeedbackLastPurchaseEmptyTemplate) +
                GetDeviceInfoTemplate();

            var expectedUnescapedMessage = UnescapeMessage(expectedMessage);
            var unescapedMessage = UnescapeMessage(message);
            unescapedMessage.Should().Be(expectedUnescapedMessage);
        }

        private static void EnsureFeedbackMessageForUnauthorizedUserHasExpectedContent(Feedback feedback, string message)
        {
            var expectedMessage =
                GetFeedbackContentTemplate() +
                GetPhoneNumberTemplate() +
                GetNoInfoTemplate(FeedbackRussianMessageContents.FeedbackPosEmptyTemplate) +
                GetDateCreatedTemplate(feedback) +
                GetNoInfoTemplate(FeedbackRussianMessageContents.FeedbackLastPurchaseEmptyTemplate) +
                GetDeviceInfoTemplate();

            var expectedUnescapedMessage = UnescapeMessage(expectedMessage);
            var unescapedMessage = UnescapeMessage(message);
            unescapedMessage.Should().Be(expectedUnescapedMessage);
        }

        private static string GetFeedbackContentTemplate()
        {
            return string.Format(FeedbackRussianMessageContents.FeedbackContentTemplate, FeedbackRussianMessageContents.FeedbackContent);
        }

        private static string GetPhoneNumberTemplate()
        {
            return string.Format(FeedbackRussianMessageContents.FeedbackPhoneNumberTemplate,
                FeedbackRussianMessageContents.UserPhoneNumber);
        }

        private static string GetNoInfoTemplate(string label)
        {
            return string.Format(label, FeedbackRussianMessageContents.NoInfoLabel);
        }

        private static string GetNoPurchasesYetTemplate(string label)
        {
            return string.Format(label, FeedbackRussianMessageContents.NoPurchasesYetLabel);
        }

        private static string GetAuthorizedUserFeedbackHeaderTemplate()
        {
            return GetFeedbackContentTemplate() +
                   string.Format(FeedbackRussianMessageContents.FeedbackAuthorizedUserInfoTemplate,
                       FeedbackRussianMessageContents.UserFullName,
                       DefaultUserId,
                       string.Format(FeedbackRussianMessageContents.AdminPageLinkFakeFormat["UsersListPage"],
                           DefaultUserId)) +
                   GetPhoneNumberTemplate();
        }

        private static string GetDeviceInfoTemplate()
        {
            return string.Format(FeedbackRussianMessageContents.FeedbackDeviceInfoTemplate,
                FeedbackRussianMessageContents.DeviceName,
                FeedbackRussianMessageContents.DeviceOperatingSystem,
                FeedbackRussianMessageContents.AppVersion);
        }
        private static string GetDateCreatedTemplate(Feedback feedback)
        {
            return string.Format(FeedbackRussianMessageContents.FeedbackDateCreatedTemplate,
                GetFeedBackCreatingDateTimeInMoscowTimeZoneTemplate(feedback.DateCreated));
        }
        private static string GetPosInfoTemplate()
        {
            return string.Format(FeedbackRussianMessageContents.FeedbackPosTemplate,
                FeedbackRussianMessageContents.PosName,
                string.Format(FeedbackRussianMessageContents.AdminPageLinkFakeFormat["PosDetailsPage"],
                    DefaultPosId));
        }
        private static string GetCheckInfoTemplate()
        {
            return string.Format(FeedbackRussianMessageContents.FeedbackLastPurchaseTemplate,
                FeedbackRussianMessageContents.GoodName,
                $"{DefaultSecondPrice:0.00} {FeedbackRussianMessageContents.CurrencyName}",
                string.Empty,
                DefaultSecondPosOperationId);
        }

        private static string GetPurchaseDateTemplate(Feedback feedback)
        {
            return string.Format(FeedbackRussianMessageContents.FeedbackPurchaseDateTemplate,
                GetFeedBackCreatingDateTimeInMoscowTimeZoneTemplate(feedback.DateCreated));
        }

        private static string GetUserBalanceTemplate(decimal balance)
        {
            return string.Format(FeedbackRussianMessageContents.FeedbackUserBalanceTemplate,
                $"{balance:0.00} {FeedbackRussianMessageContents.CurrencyName}");
        }

        private static string UnescapeMessage(string message)
        {
            return message
                .Replace("\r\n", "")
                .Replace(Environment.NewLine, "");
        }

        private void EnsureExpectedFeedbackEntityCreated(ExpectedFeedbackEntity expectedFeedback)
        {
            var feedback = GetFeedbackEntityByIdAsync(AddedFeedbackEntityId).Result;

            feedback.Should().NotBeNull();

            feedback.AppVersion.Should().Be(expectedFeedback.AppVersion);
            feedback.Content.Should().Be(expectedFeedback.Content);
            feedback.DateCreated.Date.Should().Be(expectedFeedback.DateCreated);
            feedback.DeviceName.Should().Be(expectedFeedback.DeviceName);
            feedback.DeviceOperatingSystem.Should().Be(expectedFeedback.DeviceOperatingSystem);
            feedback.PhoneNumber.Should().Be(expectedFeedback.PhoneNumber);
            feedback.PosId.Should().Be(expectedFeedback.PosId);
            feedback.UserId.Should().Be(expectedFeedback.UserId);
        }

        private void SeedPaidPosOperationsWithCheckItems()
        {
            var operation1 = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetCheckItems(new List<CheckItem>
                {
                    CreateCheckItem(DefaultPosId, CheckItemStatus.Paid, DefaultFirstLabeledGoodId, DefaultFirstLPosOperationId, DefaultFirstPrice)
                })
                .MarkAsPendingPayment()
                .MarkAsPaid(OperationPaymentInfo.ForPaymentViaBonuses(DefaultUserId, DefaultFirstPrice))
                .Build();

            var operation2 = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetCheckItems(new List<CheckItem>
                {
                    CreateCheckItem(DefaultPosId, CheckItemStatus.Paid, DefaultSecondLabeledGoodId, DefaultSecondPosOperationId, DefaultSecondPrice)
                })
                .MarkAsPendingPayment()
                .MarkAsPaid(OperationPaymentInfo.ForPaymentViaBonuses(DefaultUserId, DefaultSecondPrice))
                .Build();

            Seeder.Seed(new List<PosOperation> { operation1, operation2 });
        }

        private void SeedCompletedPosOperationsWithUnpaidCheckItems()
        {
            var operation1 = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetCheckItems(new List<CheckItem>
                {
                    CreateCheckItem(DefaultPosId, CheckItemStatus.Unpaid, DefaultFirstLabeledGoodId, DefaultFirstLPosOperationId, DefaultFirstPrice)
                })
                .MarkAsCompleted()
                .Build();

            var operation2 = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetCheckItems(new List<CheckItem>
                {
                    CreateCheckItem(DefaultPosId, CheckItemStatus.Unpaid, DefaultSecondLabeledGoodId, DefaultSecondPosOperationId, DefaultSecondPrice)
                })
                .MarkAsCompleted()
                .Build();

            Seeder.Seed(new List<PosOperation> { operation1, operation2 });
        }

        private void SeedPosOperationsWithNoChecks()
        {
            var operation1 = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsCompleted()
                .Build();

            var operation2 = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .MarkAsCompleted()
                .Build();

            Seeder.Seed(new List<PosOperation> { operation1, operation2 });
        }

        private void SeedOpenedPosOperations()
        {
            var operation1 = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .Build();

            var operation2 = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .Build();

            Seeder.Seed(new List<PosOperation> { operation1, operation2 });
        }

        private static CheckItem CreateCheckItem(int posId, CheckItemStatus status, int labeledGoodId, int posOperationId, decimal price)
        {
            return CheckItem.NewBuilder(
                    posId,
                    posOperationId,
                    1,
                    labeledGoodId,
                    1)
                .SetPrice(price)
                .SetStatus(status)
                .Build();
        }

        private static ApplicationUser GetFixtureUser()
        {
            return new ApplicationUser
            {
                Id = DefaultUserId,
                PhoneNumber = FeedbackRussianMessageContents.UserPhoneNumber,
                UserName = FeedbackRussianMessageContents.UserFullName
            };
        }

        private static FeedbackDto CreateFixtureFeedbackDto()
        {
            return new FeedbackDto
            {
                SenderInfo = new SenderInfoDto
                {
                    DeviceInfo = new DeviceInfoDto
                    {
                        DeviceName = FeedbackRussianMessageContents.DeviceName,
                        OperatingSystem = FeedbackRussianMessageContents.DeviceOperatingSystem
                    },
                    PhoneNumber = FeedbackRussianMessageContents.UserPhoneNumber
                },
                AppInfo = new AppInfoDto
                {
                    AppVersion = FeedbackRussianMessageContents.AppVersion
                },
                Body = new FeedbackBodyDto
                {
                    Content = FeedbackRussianMessageContents.FeedbackContent
                }
            };
        }

        private Feedback GetFixtureFeedback(ApplicationUser user)
        {
            var deviceInfo = new DeviceInfo(FeedbackRussianMessageContents.DeviceName, FeedbackRussianMessageContents.DeviceOperatingSystem);
            var senderInfo = _senderInfoFactory.CreateAsync(new SenderCreationInfo(user, deviceInfo)).Result;
            var feedbackBody = new FeedbackBody(FeedbackRussianMessageContents.FeedbackContent);
            var appInfo = new AppInfo(FeedbackRussianMessageContents.AppVersion);

            return Feedback.NewInstance(senderInfo, feedbackBody, appInfo);
        }

        private Feedback GetFixtureFeedbackForUnauthorizedUser()
        {
            var deviceInfo = new DeviceInfo(FeedbackRussianMessageContents.DeviceName, FeedbackRussianMessageContents.DeviceOperatingSystem);
            var authorizedSenderInfo = _senderInfoFactory.CreateAsync(new SenderCreationInfo(FeedbackRussianMessageContents.UserPhoneNumber, deviceInfo)).Result;
            var feedbackBody = new FeedbackBody(FeedbackRussianMessageContents.FeedbackContent);
            var appInfo = new AppInfo(FeedbackRussianMessageContents.AppVersion);

            return Feedback.NewInstance(authorizedSenderInfo, feedbackBody, appInfo);
        }

        private static string GetFeedBackCreatingDateTimeInMoscowTimeZoneTemplate(DateTime feedbackDateCreated)
        {
            var moscowDateTime = UtcMoscowDateTimeConverter.ConvertToMoscowDateTime(feedbackDateCreated);
            return SharedDateTimeConverter.ConvertDateHourMinutePartsToString(moscowDateTime);
        }

        private async Task<FeedbackEntity> GetFeedbackEntityByIdAsync(int id)
        {
            using (var context = _dbContextFactory.Create())
            {
                return await context.Feedbacks.SingleOrDefaultAsync(g => g.Id == id);
            }
        }
    }
}
