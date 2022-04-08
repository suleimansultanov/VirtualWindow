using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Services.MessageSender.Sms;
using NasladdinPlace.Core.Services.MessageSender.Sms.Contracts;
using NasladdinPlace.Core.Services.MessageSender.Sms.Models;
using NasladdinPlace.Core.Services.NotificationsLogger;
using NasladdinPlace.Core.Services.WebClient.Contracts;
using NasladdinPlace.Core.Services.WebClient.Models;
using NasladdinPlace.Logging;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NUnit.Framework;
using System;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Tests.Scenarios.Sms
{
    public class SmsRuMessageSenderIntegrationTests : TestsBase
    {
        private const int DefaultUserId = 1;
        private const string DefaultApiId = "BF084B1C-B946-A757-3B04-56156AC6DBD3";
        private const string DefaultUrl = "https://sms.ru/sms/send";
        private const string DefaultFromSender = "TestNasladdin";
        private const string DefaultPhoneNumber = "79857125410";
        private const string IncorrectPhoneNumber = "00000000000";
        private const int DefaultSmsServiceMinimumBalance = 1000;
        private readonly string IncorrectApiId = Guid.NewGuid().ToString();

        private ISmsSender _smsSender;
        private INotificationsLogger _notificationsLogger;
        private IJsonWebClient _webClient;
        private ILogger _logger;

        public override void SetUp()
        {
            base.SetUp();
            Seeder.Seed(new UsersDataSet());

            var serviceProvider = TestServiceProviderFactory.Create();

            _webClient = serviceProvider.GetRequiredService<IJsonWebClient>();
            _notificationsLogger = serviceProvider.GetRequiredService<INotificationsLogger>();
            _logger = serviceProvider.GetRequiredService<ILogger>();
        }

        [Test]
        public void SendSmsAsync_CorrectParametersAreGiven_ShouldReturnOkResult()
        {
            InitSmsMessageSender(DefaultApiId);

            var smsInfo = new SmsLoggingInfo
            {
                UserId = DefaultUserId,
                Message = "test",
                PhoneNumber = DefaultPhoneNumber
            };

            var result = _smsSender.SendSmsAsync(smsInfo).Result;

            AssertSmsMessageSentResult(actualIsSentResult: result, expectedIsSentResult: true, expectedNotificationCount: 1);
        }

        [Test]
        public void SendSmsAsync_IncorrectPhoneNumberIsGiven_ShouldReturnSmsErrorResult()
        {
            InitSmsMessageSender(DefaultApiId);

            var smsInfo = new SmsLoggingInfo
            {
                UserId = DefaultUserId,
                Message = "test",
                PhoneNumber = IncorrectPhoneNumber
            };

            var result = _smsSender.SendSmsAsync(smsInfo).Result;

            AssertSmsMessageSentResult(actualIsSentResult: result, expectedIsSentResult: false, expectedNotificationCount: 1);
        }

        [Test]
        public void SendSmsAsync_IncorrectApiIdIsGiven_ShouldReturnSmsGatewayErrorResult()
        {
            InitSmsMessageSender(IncorrectApiId);

            var smsInfo = new SmsLoggingInfo
            {
                UserId = DefaultUserId,
                Message = "test",
                PhoneNumber = DefaultPhoneNumber
            };

            var result = _smsSender.SendSmsAsync(smsInfo).Result;

            AssertSmsMessageSentResult(actualIsSentResult: result, expectedIsSentResult: false, expectedNotificationCount: 1);
        }

        [Test]
        public void SendSmsAsync_ServerIsNotAvailable_ShouldReturnFalseSmsSendResult()
        {
            var mockWebClient = new Mock<IJsonWebClient>();
            mockWebClient.Setup(p => p.PerformGetRequestAsync<SmsResponseStatus>(It.IsAny<string>()))
                .Returns(
                    Task.FromResult(RequestResult<SmsResponseStatus>.Failure("error")));

            var setting = new SmsRuApiSettings(
                apiId: DefaultApiId,
                url: DefaultUrl,
                isTestEnvironment: true,
                minimumPositiveBalance: DefaultSmsServiceMinimumBalance,
                fromSender: DefaultFromSender);

            _smsSender = new SmsRuMessageSender(mockWebClient.Object, _notificationsLogger, _logger, setting);

            var smsInfo = new SmsLoggingInfo
            {
                UserId = DefaultUserId,
                Message = "test",
                PhoneNumber = DefaultPhoneNumber
            };

            var result = _smsSender.SendSmsAsync(smsInfo).Result;

            AssertSmsMessageSentResult(actualIsSentResult: result, expectedIsSentResult: false, expectedNotificationCount: 0);
        }

        private void InitSmsMessageSender(string apiId)
        {
            var setting = new SmsRuApiSettings(
                apiId: apiId,
                url: DefaultUrl,
                isTestEnvironment: true,
                minimumPositiveBalance: DefaultSmsServiceMinimumBalance,
                fromSender: DefaultFromSender);

            _smsSender = new SmsRuMessageSender(_webClient, _notificationsLogger, _logger, setting);
        }

        private void AssertSmsMessageSentResult(bool actualIsSentResult, bool expectedIsSentResult,
            int expectedNotificationCount)
        {
            actualIsSentResult.Should().Be(expectedIsSentResult);
            Context.UserNotifications.Count().Should().Be(expectedNotificationCount);
        }
    }
}