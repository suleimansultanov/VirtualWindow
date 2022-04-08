using AutoMapper;
using FluentAssertions;
using Google;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.OperationsManager;
using NasladdinPlace.Core.Services.Spreadsheet.Contracts;
using NasladdinPlace.Core.Services.Spreadsheet.Providers.Contracts;
using NasladdinPlace.Spreadsheets.Factories.Services.Spreadsheet;
using NasladdinPlace.Spreadsheets.Services.Credential.Contracts;
using NasladdinPlace.Spreadsheets.Services.Fetcher.Contracts;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Spreadsheet.Uploader.Contracts;
using NasladdinPlace.Spreadsheets.Services.Creators.Contracts;
using NasladdinPlace.Spreadsheets.Services.Formatters.Contracts;

namespace NasladdinPlace.Api.Tests.Scenarios.Spreadsheet
{
    public class SpreadsheetUploaderIntegrationTests : TestsBase
    {
        private const string TestSpreadsheetUrl =
            "https://docs.google.com/spreadsheets/d/1_L2fBQSYXwl4YRPkSxwshWwLflezG8JzhiSFPl2VhtQ/edit#gid=0";

        private const string TestSheetName = "TestSheet";
        private const int TestBatchSize = 300;

        private const int DefaultPosId = 1;
        private const int DefaultPosOperationId = 1;
        private const int DefaultUserId = 1;
        private const int DefaultPaymentCardId = 1;
        private const int DefaultTransactionId = 1;
        private const int DefaultCurrencyId = 1;
        private const int DefaultGoodId = 1;
        private const int DefaultLabeledGoodId = 1;

        private ISpreadsheetsUploader _spreadsheetsUploader;
        private Mock<ISpreadsheetProvider> _mockSpreadsheetProvider;
        private IServiceProvider _serviceProvider;
        private ISpreadsheet _spreadsheet;

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

            Mapper.Reset();
            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));

            _serviceProvider = TestServiceProviderFactory.Create();

            _mockSpreadsheetProvider =
                _serviceProvider.GetRequiredService<Mock<ISpreadsheetProvider>>();

            _spreadsheetsUploader = _serviceProvider.GetRequiredService<ISpreadsheetsUploader>();
            _spreadsheetsUploader = _serviceProvider.GetRequiredService<ISpreadsheetsUploader>();

            _spreadsheet = new Spreadsheets.Services.Spreadsheets.Spreadsheet(
                new GoogleSpreadsheetServiceFactory(_serviceProvider.GetRequiredService<IGoogleCredential>(), "NasladdinSpreadsheetsProject").Create(),
                _serviceProvider.GetRequiredService<ISpreadsheetIdFetcher>(), 
                _serviceProvider.GetRequiredService<ISpreadsheetCellFormatter>(), 
                _serviceProvider.GetRequiredService<ISpreadsheetDataRangeCreator>(), 
                TestSpreadsheetUrl);

            _spreadsheet.ClearAsync(TestSheetName).Wait();
        }

        [TestCase(CheckItemStatus.Paid, 2)]
        [TestCase(CheckItemStatus.Deleted, 1)]
        [TestCase(CheckItemStatus.PaidUnverified, 2)]
        [TestCase(CheckItemStatus.Refunded, 1)]
        [TestCase(CheckItemStatus.Unpaid, 2)]
        [TestCase(CheckItemStatus.Unverified, 1)]
        public void
            UploadAsync_CorrectPosOperationWithDifferentCheckItemAndReportInfoAreGiven_ShouldUploadDataToTable(CheckItemStatus inputCheckItemStatus, int expectedResultCount)
        {
            Seeder.Seed(new PaymentCardsDataSet(DefaultUserId));
            Seeder.Seed(new List<ReportUploadingInfo>
            {
                new ReportUploadingInfo(TestSpreadsheetUrl, ReportType.DailyPurchaseStatistics,
                    string.Empty, TestSheetName, TestBatchSize)
            });

            SetupTestPosOperation(posOperationBuilder =>
            {
                var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, 3M);
                var operationPaymentInfo =
                    OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 2M);

                var defaultCheckItem =
                    CreateCheckItemFromPosOperationIdAndLabeledGoodId(inputCheckItemStatus, 5M);

                posOperationBuilder
                    .SetCheckItems(new List<CheckItem> {defaultCheckItem})
                    .MarkAsPendingPayment()
                    .MarkAsPaid(operationPaymentInfo);
            });

            _mockSpreadsheetProvider
                .Setup(s => s.Provide(It.IsAny<string>()))
                .Returns(_spreadsheet);

            _spreadsheetsUploader.ErrorHandler += (sender, error) => { Assert.Fail(); };

            _spreadsheetsUploader.UploadAsync(ReportType.DailyPurchaseStatistics).Wait();

            var result = _spreadsheet.ReadAsync(TestSheetName).Result;
            result.Should().HaveCount(expectedResultCount);
        }

        [TestCase((int)HttpStatusCode.InternalServerError)]
        [TestCase((int)HttpStatusCode.ServiceUnavailable)]
        public void
            UploadAsync_GoogleServerUnavailability_ShouldTryRetryUploadAndThrowGoogleApiException(int expectedStatusCode)
        {
            Seeder.Seed(new PaymentCardsDataSet(DefaultUserId));
            Seeder.Seed(new List<ReportUploadingInfo>
            {
                new ReportUploadingInfo(TestSpreadsheetUrl, ReportType.DailyPurchaseStatistics,
                    string.Empty, TestSheetName, TestBatchSize)
            });

            SetupTestPosOperation(posOperationBuilder =>
            {
                var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, 3M);
                var operationPaymentInfo =
                    OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 2M);

                var defaultCheckItem =
                    CreateCheckItemFromPosOperationIdAndLabeledGoodId(CheckItemStatus.Paid, 5M);

                posOperationBuilder
                    .SetCheckItems(new List<CheckItem> {defaultCheckItem})
                    .MarkAsPendingPayment()
                    .MarkAsPaid(operationPaymentInfo);
            });

            var mockSpreadsheet = new Mock<ISpreadsheet>();

            var calls = 0;

            mockSpreadsheet.Setup(s => s.ClearAsync(It.IsAny<string>()))
                .Callback(() =>
                {
                    calls++;

                    var error = new GoogleApiException("TestException", "TestExceptionMessage");

                    var requestError = new Google.Apis.Requests.RequestError
                    {
                        Code = expectedStatusCode,
                        Message = "TestExceptionMessage"
                    };

                    error.Error = requestError;
                    throw error;
                });

            _mockSpreadsheetProvider
                .Setup(s => s.Provide(It.IsAny<string>()))
                .Returns(mockSpreadsheet.Object);

            var errorResultsCount = 0;
            _spreadsheetsUploader.ErrorHandler += (sender, error) =>
            {
                error.Code.Should().Be(expectedStatusCode);
                error.Message.Should().BeEquivalentTo("TestExceptionMessage");
                Interlocked.Increment(ref errorResultsCount);
            };

            _spreadsheetsUploader.UploadAsync(ReportType.DailyPurchaseStatistics).Wait();
          
            calls.Should().BeGreaterThan(0);
            AssertIntegersEquals(errorResultsCount, 1);
        }

        [Test]
        public void
            UploadAsync_CorrectInputDataAreGivenButErrorOccurredOnBackend_ThrowException()
        {
            Seeder.Seed(new PaymentCardsDataSet(DefaultUserId));
            Seeder.Seed(new List<ReportUploadingInfo>
            {
                new ReportUploadingInfo(TestSpreadsheetUrl, ReportType.PointsOfSaleContent,
                    string.Empty, TestSheetName, TestBatchSize)
            });

            SetupTestPosOperation(posOperationBuilder =>
            {
                var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, 3M);
                var operationPaymentInfo =
                    OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 2M);

                var defaultCheckItem =
                    CreateCheckItemFromPosOperationIdAndLabeledGoodId(CheckItemStatus.Paid, 5M);

                posOperationBuilder
                    .SetCheckItems(new List<CheckItem> { defaultCheckItem })
                    .MarkAsPendingPayment()
                    .MarkAsPaid(operationPaymentInfo);
            });

            var mockSpreadsheet = new Mock<ISpreadsheet>();

            mockSpreadsheet.Setup(s => s.ClearAsync(It.IsAny<string>()))
                .Callback(() => throw new Exception("TestExceptionMessage"));

            _mockSpreadsheetProvider
                .Setup(s => s.Provide(It.IsAny<string>()))
                .Returns(mockSpreadsheet.Object);

            var errorResultsCount = 0;

            _spreadsheetsUploader.ErrorHandler += (sender, error) =>
            {
                error.Code.Should().Be(default(int));
                error.Message.Should().BeEquivalentTo("TestExceptionMessage");
                Interlocked.Increment(ref errorResultsCount);
            };

            _spreadsheetsUploader.UploadAsync(ReportType.PointsOfSaleContent).Wait();

            AssertIntegersEquals(errorResultsCount, 1);
        }

        [TestCase(0)]
        [TestCase(1)]   
        public void
            UploadAsync_SingleGoogleServerUnavailability_ShouldUploadData(int inputCallsCount)
        {
            Seeder.Seed(new PaymentCardsDataSet(DefaultUserId));
            Seeder.Seed(new List<ReportUploadingInfo>
            {
                new ReportUploadingInfo(TestSpreadsheetUrl, ReportType.PointsOfSaleContent,
                    string.Empty, TestSheetName, TestBatchSize)
            });

            SetupTestPosOperation(posOperationBuilder =>
            {
                var bankTransactionSummary = new BankTransactionSummary(DefaultPaymentCardId, DefaultTransactionId, 3M);
                var operationPaymentInfo =
                    OperationPaymentInfo.ForMixPayment(DefaultUserId, bankTransactionSummary, 2M);

                var defaultCheckItem =
                    CreateCheckItemFromPosOperationIdAndLabeledGoodId(CheckItemStatus.Paid, 5M);

                posOperationBuilder
                    .SetCheckItems(new List<CheckItem> { defaultCheckItem })
                    .MarkAsPendingPayment()
                    .MarkAsPaid(operationPaymentInfo);
            });

            var mockSpreadsheet = new Mock<ISpreadsheet>();

            var calls = 0;

            mockSpreadsheet.Setup(s => s.ClearAsync(It.IsAny<string>()))
                .Returns(() => Task.CompletedTask)
                .Callback(() =>
                {
                    calls++;

                    if (calls > inputCallsCount) return;

                    var error = new GoogleApiException("TestException", "TestExceptionMessage");

                    var requestError = new Google.Apis.Requests.RequestError
                    {
                        Code = (int) HttpStatusCode.InternalServerError,
                        Message = "TestExceptionMessage"
                    };

                    error.Error = requestError;
                    throw error;
                });

            _mockSpreadsheetProvider
                .Setup(s => s.Provide(It.IsAny<string>()))
                .Returns(mockSpreadsheet.Object);

            var errorResultsCount = 0;

            _spreadsheetsUploader.ErrorHandler += (sender, error) =>
            {
                Interlocked.Increment(ref errorResultsCount);
            };

            _spreadsheetsUploader.UploadAsync(ReportType.PointsOfSaleContent).Wait();

            calls.Should().Be(inputCallsCount + 1);
            AssertIntegersEquals(errorResultsCount, 0);
        }

        private void AssertIntegersEquals(int errorResultCount ,int expectedCount)
        {
            Interlocked.CompareExchange(ref errorResultCount, expectedCount, expectedCount)
                .Should()
                .Be(expectedCount);
        }

        private static CheckItem CreateCheckItemFromPosOperationIdAndLabeledGoodId(CheckItemStatus status,
            decimal price, bool isModifiedByAdmin = false)
        {
            var checkItemBuilder = CheckItem.NewBuilder(
                    DefaultPosId,
                    DefaultPosOperationId,
                    DefaultGoodId,
                    DefaultLabeledGoodId,
                    DefaultCurrencyId)
                .SetPrice(price)
                .SetStatus(status);

            if (isModifiedByAdmin)
                checkItemBuilder.MarkAsModifiedByAdmin();

            return checkItemBuilder.Build();
        }

        private void SetupTestPosOperation(Action<PosOperationOfUserAndPosBuilder> additionalInitializationFunc = null)
        {
            var posAddressCoordinates = new Location(10, 11);
            var posAddress = Address.FromCityStreetAtCoordinates(1, "Тыныстанова 14", posAddressCoordinates);
            var operationOfUserAndPosBuilder = PosOperation.NewOfUserAndPosBuilder(DefaultUserId, DefaultPosId)
                .SetPos(new Pos(DefaultPosId, "Витрина 2", "Test2", posAddress)
                {
                    QrCode = "9C9FA2DD-1CCC-4885-8D3D-86F19123261B"
                });

            additionalInitializationFunc?.Invoke(operationOfUserAndPosBuilder);

            var posOperation = operationOfUserAndPosBuilder.Build();

            Seeder.Seed(new List<PosOperation> { posOperation });
        }
    }
}
