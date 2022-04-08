using CloudPaymentsClient.Domain.Factories.PaymentService;
using FluentAssertions;
using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Core.Services.Check.Helpers;
using NasladdinPlace.Payment.Models;
using NasladdinPlace.Payment.Services;
using NUnit.Framework;

namespace CloudPaymentsClient.Tests
{
    public class CloudPaymentsServiceTests
    {
        private const string ServiceId = "pk_d60a2fee7b77994ebee662cb2b6a6";
        private const string ServiceSecret = "bffbabb12e1eb01eed2096ec429778ba";
        
        private const string CryptogramCardVisa3DsSuccessfulPayment = "014242424242221102glM5qc0yCrKfq+XvYCJy/tpgUjtnNII0tMKESYJvTgaelrEdfxl4cQRNt4Jz6+Nu/FeXWmLvioKlB22mcdN6aXYxK9im1a9IhAFFQL7cD97cdAYOaJMeFr70lsJT9fw67CkZR63mx7Z3XioH9SkOT4y7ecLmQ2N4qEoel9AskewbBtHEsDGJ+B7hprViPCXhwVx5jfNMxFekkFxl63dO8bcYg4rwh19YxXo0py/xPesGjQFR6VmPiVR+ssUMSCQUp3urauD4SRRTslSEzu+uvFbuNbemw7/ZWD249qhfWqz1CQR3bXA32V6xilzOFKgpFCLGOE6oeOe5mSCWrZw/QA==";
        private const string CryptogramCardMasterCard3DsSuccessfulPayment = "015555554444221102JFtgERDOmzKiko6AevcUQb8B330oBvgei299w7wvvNikihyIb+WZMvsXcRI++7p/YSKM43rsUXU9XcfWIj4NRtKnR5L+aXvrMJjhd/V0AHxJxNS76Td6xLhJYeHdKOGgWayOvuZSXUPoCSONK1sW5PDumn+P7DovfvDpLfoJ4S7LoIMHhv2TKxlS7/T6G+azgo7iyqicRCFOuwj23tZz1+5M7gvbzJKTY5j5P8GxwU23mw3FScT9d6FIqSyLgwdxooWJ1AWEGLT4toU2K49KSlsTaA0vMwJrQKVGqEjFKRG96HVwpshbyLun93J85JOuuAkGe1Y0TZNK3OumL/OATw==";
        private const string CryptogramCardVisa3DsInsufficientFunds = "014012881881221102SfGSmb6xpLxgCV+fXbElT1v0wNm6yd3LOilIjWTaOmWczaehVCdy09x/iaE5+FYm/Jt/KeKzZMYigE6Y9v+pyygip4rp1YA7ahm7PPXr1Y+P+iULIP1EViALHfG6iSGcy16lGggkwKgBkm/2arSRDuF325oKhKdPIHkzuVSBEazUBTJxwH7TEdLsbkkEFeJIe7JK6gm++lSAnk+EEEwfVwO2P2DDrQWwWgacB+tUJ9P/jRR7TMogb2oXNF9HQbxg1kIfVTzHvwUWLhNlBpbFiC/Lxin9rVpnxsdqnXmo1lu9X430ICFkoBBPktdM1hhJf+4WQWv09TeQ6VedKvKcxA==";
        private const string CryptogramCardMasterCard3DsInsufficientFunds = "015105105100221102dNDIt7+75S3JY2q/zHVf15qvk6DkoHXmt/OUr+hBiVsS+XRkRmnq6UDXruuqJkf2xgwgiCoNnGj0HIO8U9szCpDr84QHHRCKIsAvNqfufxLWWQ2hcmYXALskGJ+kOTG+LcBGQuwCfOGgBEIjaDKNi8LqaTpw5zBsJeraSu4u589L2JUunn0xparjkIXb69t31m9pZjUfTpFztZkEaUsvV4Fu6FqCf+Z+VmeCYfztYFKj6n58ObZVDuSi6KK2p40k8ueoM+wE+UhmWlhAItfewos2oM8iSETK+DmzLVzoWZorLH7P6LA0QcqEZ6dZC/x8Xgt7905ryt0Q09hhwZatHw==";
        private const string CryptogramCardVisaSuccessfulPayment = "014111111111221102IuUDalV2SXohVf9+dGtsH+hI8W/mgbDyKH9KFZnQTGa/8oSCaFEF54aqR3rpwanlfcRyQhM7ZTebXuRw8vgWvdrkgerDLlcJ2UfVylVHVPUI4TrQ0qGhyx9JmeqGfFlS2vwltZT5IBI9Ci7KAlcuCBHgUc4/epKl3+0Km92LqfBRn0H14spyUT8SZUvVGJOB5eJBlyLzyoQII125KFKc/4fzB8lXTO4Wgj5uXDJcvK/jNh+3TIdXhadUbzw/G+pwsOXNex9XG0gLaDwFslw539PNYXjMK+Ubbdi4KCmZOCYw9GIGSdYeeu/+0pmA/FniWNgJz6XA/6A5KSWxjKHvtw==";
        private const string CryptogramCardMasterCardSuccessfulPayment = "015200828210221102gCBD/gnsVGyBszpkn89/LpE0WUOGEpALM3143Fn2Ud4htLRlDJiig/UOImIO3mPMbh6wk/sN/DwwKrEXMBDllkznC9SpcGLqCB8zjyU65Q6e2bH7S65Qb3h3snqJwitLmQkWqL8Dy9XGQQEGiNaNzomtjeilwjb+9QQXuPQzFHiAwL7SGRg9ZaV3+7bnXtsnx2sQFsRJDAC4RCBZ0JLKV9No/uCllKgQc8j9gP9q4kUX1lOBhkBU6jCXdb+b3CqoMmD1Y9AUDIGx+ICcdnaOT+Oif3pYZZmuMHllHIwBeBQqod87I8SN9xvB2WJOJP/Rmt4FwDw2oJokbQl6Trbk7g==";
        private const string CryptogramCardVisaInsufficientFunds = "014000055556221102Y4asdFONN/kPBJSWVQb5kpiwDj8mGjlmywxAzGu5xB9Ye/31SRlgNLfpjW7Ox2SV0bweM71xbblzicfyWtTampLPgroYhyWYmvWDasV66Dp4nseT4RQfU+/8XQVqgOopzvnraMRaNZRk1WGQd56P3VNz/NUPTwVGLltadf246IfUuAglvaIxlx2g9M+9AtcCqq2kkrHRmsNRia3FmNGDbec76xNCTzmBVHia2EauIh9t/v4YzBYnQ9O4HusoFRkzeNPNcxYRZK6ZMuyhXKJROBwJrzRb4civi7upRP5n+3XyhkQdcXWGrmbqU9kRLknM0JaWCMHCJ2Qm7r9MXMVp0Q==";
        private const string CryptogramCardMasterCardInsufficientFunds = "015404000043221102A9Lj5dG1bkd482AyLBBbdv8Zq7qi7vPrEd9z1lERH9g5zETU8vIc5W5Ap7lkeRpRGSu2hdfpSedbiAt0zfJhswjkTeHsTb7nzpWNY87yMjRpweqthj7VlM+eqg8cPQbQHBwAxxaFnt1HbUCnqRP2wZtkvMzNrh9z/bkfvRyR7EnJMHHvr3Oleb/7CHigJdxvM3IMwXRiGAwi6BH6fkcLGqcH5MKKJ6KXMmcVm4oI6NNi9qxXub8wS9Gw1dqByKR9DFsaftQjVgZv2fEQ9zmgCSOtgQYF4rDiRjHwrH70E/2NDpAi8IY0U59ru+i/FnXpzRV/2eE049CekEdGeUOdxg==";
        private const string IncorrectCryptogram = "fdjdaosjf;kasdjf;alsdkjfad";

        private const string CardHolder = "TEST USER";

        private const string UserIpAddress = "192.168.1.122";

        private const string UserIdentifier = "user_1";
        
        private const Currency Currency = NasladdinPlace.Payment.Models.Currency.Rubles;
        private const string PaymentTestDescription = "test";

        private IPaymentService _paymentService;
        
        [SetUp]
        public void SetUp()
        {
            var serviceInfo = new ServiceInfo(ServiceId, ServiceSecret);
            _paymentService = new CloudPaymentsesServiceFactory(serviceInfo).CreatePaymentService();
        }

        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 1)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 100)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 1000)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 10000)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 1)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 100)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 1000)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 10000)]
        public void MakePaymentAsync_ValidBankingCardIsGiven_ShouldReturnPaidResponse(
            string cardCryptogram, decimal paymentAmount)
        {
            var paymentRequest = new PaymentRequest(
                paymentAmount, Currency, cardCryptogram, CardHolder, UserIpAddress, UserIdentifier)
            {
                Description = PaymentTestDescription
            };

            var paymentResult = _paymentService.MakePaymentAsync(paymentRequest).Result;

            paymentResult.IsSuccess.Should().BeTrue();
            paymentResult.Error.Should().BeNullOrEmpty();
            paymentResult.Status.Should().Be(ResponseStatus.Success);
            paymentResult.Result.Info3Ds.Should().BeNull();
            paymentResult.Result.PaymentStatus.Should().Be(PaymentStatus.Paid);
            paymentResult.Result.TransactionId.Should().NotBe(0);
            paymentResult.Result.Error.Should().BeEmpty();
            paymentResult.Result.PaymentCardInfo.Token.Should().NotBeNullOrWhiteSpace();
        }

        [TestCase(CryptogramCardMasterCardInsufficientFunds, 1)]
        [TestCase(CryptogramCardMasterCardInsufficientFunds, 100)]
        [TestCase(CryptogramCardMasterCardInsufficientFunds, 1000)]
        [TestCase(CryptogramCardMasterCardInsufficientFunds, 10000)]
        [TestCase(CryptogramCardVisaInsufficientFunds, 1)]
        [TestCase(CryptogramCardVisaInsufficientFunds, 100)]
        [TestCase(CryptogramCardVisaInsufficientFunds, 1000)]
        [TestCase(CryptogramCardVisaInsufficientFunds, 10000)]
        public void MakePaymentAsync_InsufficientFundsBankingCardIsGiven_ShouldReturnUnpaidResponse(
            string cardCryptogram, decimal paymentAmount)
        {
            var paymentRequest = new PaymentRequest(
                paymentAmount, Currency, cardCryptogram, CardHolder, UserIpAddress, UserIdentifier)
            {
                Description = PaymentTestDescription
            }; 

            var paymentResult = _paymentService.MakePaymentAsync(paymentRequest).Result;
            paymentResult.IsSuccess.Should().BeTrue();
            paymentResult.Status.Should().Be(ResponseStatus.Success);
            paymentResult.Result.Info3Ds.Should().BeNull();
            paymentResult.Result.PaymentStatus.Should().Be(PaymentStatus.NotPaid);
            paymentResult.Result.TransactionId.Should().NotBe(0);
            paymentResult.Result.Error.Should().NotBeNullOrWhiteSpace();
            paymentResult.Result.PaymentCardInfo.Should().BeNull();
        }

        [TestCase(CryptogramCardVisa3DsSuccessfulPayment, 1)]
        [TestCase(CryptogramCardVisa3DsSuccessfulPayment, 100)]
        [TestCase(CryptogramCardVisa3DsSuccessfulPayment, 1000)]
        [TestCase(CryptogramCardVisa3DsSuccessfulPayment, 10000)]
        [TestCase(CryptogramCardMasterCard3DsSuccessfulPayment, 1)]
        [TestCase(CryptogramCardMasterCard3DsSuccessfulPayment, 100)]
        [TestCase(CryptogramCardMasterCard3DsSuccessfulPayment, 1000)]
        [TestCase(CryptogramCardMasterCard3DsSuccessfulPayment, 10000)]
        public void MakePaymentAsync_BankingCardWith3DsIsGiven_ShouldReturn3DsInfo(
            string cardCryptogram, decimal paymentAmount)
        {
            var paymentRequest = new PaymentRequest(
                paymentAmount, Currency, cardCryptogram, CardHolder, UserIpAddress, UserIdentifier)
            {
                Description = PaymentTestDescription
            };

            var paymentResult = _paymentService.MakePaymentAsync(paymentRequest).Result;
            paymentResult.IsSuccess.Should().BeTrue();
            paymentResult.Status.Should().Be(ResponseStatus.Success);
            paymentResult.Result.Info3Ds.Should().NotBeNull();
            paymentResult.Result.Info3Ds.AcsUrl.Should().NotBeNullOrWhiteSpace();
            paymentResult.Result.Info3Ds.PaReq.Should().NotBeNullOrWhiteSpace();
            paymentResult.Result.PaymentStatus.Should().Be(PaymentStatus.Require3Ds);
            paymentResult.Result.TransactionId.Should().NotBe(0);
            paymentResult.Result.Error.Should().BeEmpty();
            paymentResult.Result.PaymentCardInfo.Should().BeNull();
        }

        [TestCase(IncorrectCryptogram, 1)]
        [TestCase(IncorrectCryptogram, 100)]
        [TestCase(IncorrectCryptogram, 1000)]
        [TestCase(IncorrectCryptogram, 10000)]
        public void MakePaymentAsync_IncorrectCryptogramIsGiven_ShouldReturnFailure(
            string cardCryptogram, decimal paymentAmount)
        {
            var paymentRequest = new PaymentRequest(
                paymentAmount, Currency, cardCryptogram, CardHolder, UserIpAddress, UserIdentifier)
            {
                Description = PaymentTestDescription
            };

            var paymentResult = _paymentService.MakePaymentAsync(paymentRequest).Result;
            paymentResult.IsSuccess.Should().BeFalse();
            paymentResult.Status.Should().Be(ResponseStatus.Failure);
            paymentResult.Result.Should().BeNull();
        }

        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 1)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 100)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 1000)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 10000)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 1)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 100)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 1000)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 10000)]
        public void MakeRecurrentPaymentAsync_CardWithInsufficientFundsIsGiven_ShouldReturnUnpaidResponse(
            string cardCryptogram, decimal paymentAmount)
        {
            var paymentRequest = new PaymentRequest(
                paymentAmount, Currency, cardCryptogram, CardHolder, UserIpAddress, UserIdentifier)
            {
                Description = PaymentTestDescription
            };

            var paymentResult = _paymentService.MakePaymentAsync(paymentRequest).Result;

            var cardToken = paymentResult.Result.PaymentCardInfo.Token;
            
            cardToken.Should().NotBeNullOrWhiteSpace();
            
            var recurrentPaymentRequest = new RecurrentPaymentRequest(paymentAmount, Currency, cardToken, UserIdentifier)
            {
                Description = PaymentTestDescription
            };

            var recurrentPaymentResult = _paymentService.MakeRecurrentPaymentAsync(recurrentPaymentRequest).Result;
            recurrentPaymentResult.IsSuccess.Should().BeTrue();
            recurrentPaymentResult.Status.Should().Be(ResponseStatus.Success);
            recurrentPaymentResult.Result.Info3Ds.Should().BeNull();
            recurrentPaymentResult.Result.PaymentStatus.Should().Be(PaymentStatus.NotPaid);
            recurrentPaymentResult.Result.TransactionId.Should().NotBe(0);
            recurrentPaymentResult.Result.Error.Should().NotBeNullOrWhiteSpace();
            recurrentPaymentResult.Result.PaymentCardInfo.Should().BeNull();
        }

        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 1)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 100)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 1000)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 10000)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 1)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 100)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 1000)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 10000)]
        public void MakeRefundAsync_ValidTransactionIsGiven_ShouldReturnPaidResponse(
            string cardCryptogram, decimal paymentAmount)
        {
            var paymentRequest = new PaymentRequest(
                paymentAmount, Currency, cardCryptogram, CardHolder, UserIpAddress, UserIdentifier)
            {
                Description = PaymentTestDescription
            };

            var paymentResponse = _paymentService.MakePaymentAsync(paymentRequest).Result;

            var transactionId = paymentResponse.Result.TransactionId;
            transactionId.Should().NotBe(0);
            
            var refundRequest = new RefundRequest(transactionId, paymentAmount);
            var refundResponse = CloudPaymentRepeater.MakeRefundAsync(_paymentService, refundRequest).Result;
            refundResponse.Error.Should().BeNullOrEmpty();
            refundResponse.IsSuccess.Should().BeTrue();
            refundResponse.Status.Should().Be(ResponseStatus.Success);
            refundResponse.Result.IsSuccessful.Should().BeTrue();
            refundResponse.Result.Error.Should().BeNullOrEmpty();
        }

        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 1)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 100)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 1000)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 10000)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 1)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 100)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 1000)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 10000)]
        public void AuthPaymentAsync_ValidBankingCardIsGiven_ShouldReturnPaidResponse(
            string cardCryptogram, decimal paymentAmount)
        {
            var paymentRequest = new PaymentRequest(
                paymentAmount, Currency, cardCryptogram, CardHolder, UserIpAddress, UserIdentifier)
            {
                Description = PaymentTestDescription
            };

            var paymentResult = _paymentService.AuthPaymentAsync(paymentRequest).Result;
            paymentResult.IsSuccess.Should().BeTrue();
            paymentResult.Error.Should().BeNullOrEmpty();
            paymentResult.Status.Should().Be(ResponseStatus.Success);
            paymentResult.Result.Info3Ds.Should().BeNull();
            paymentResult.Result.PaymentStatus.Should().Be(PaymentStatus.Paid);
            paymentResult.Result.TransactionId.Should().NotBe(0);
            paymentResult.Result.Error.Should().BeEmpty();
            paymentResult.Result.PaymentCardInfo.Should().NotBeNull();
        }

        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 1)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 100)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 1000)]
        [TestCase(CryptogramCardMasterCardSuccessfulPayment, 10000)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 1)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 100)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 1000)]
        [TestCase(CryptogramCardVisaSuccessfulPayment, 10000)]
        public void CancelPaymentAsync_ValidBankingCardIsGiven_ShouldReturnPaidResponse(
            string cardCryptogram, decimal paymentAmount)
        {
            var paymentRequest = new PaymentRequest(
                paymentAmount, Currency, cardCryptogram, CardHolder, UserIpAddress, UserIdentifier)
            {
                Description = PaymentTestDescription
            };

            var paymentResult = _paymentService.AuthPaymentAsync(paymentRequest).Result;
            paymentResult.IsSuccess.Should().BeTrue();
            paymentResult.Error.Should().BeNullOrEmpty();
            paymentResult.Status.Should().Be(ResponseStatus.Success);
            paymentResult.Result.Info3Ds.Should().BeNull();
            paymentResult.Result.PaymentStatus.Should().Be(PaymentStatus.Paid);
            paymentResult.Result.TransactionId.Should().NotBe(0);
            paymentResult.Result.Error.Should().BeEmpty();
            paymentResult.Result.PaymentCardInfo.Should().NotBeNull();
            
            var paymentCancellationRequest = new PaymentCancellationRequest(paymentResult.Result.TransactionId);
            var paymentCancellationResult = _paymentService.CancelPaymentAsync(paymentCancellationRequest).Result;

            paymentCancellationResult.IsSuccess.Should().BeTrue();
            paymentCancellationResult.Error.Should().BeNullOrEmpty();
            paymentCancellationResult.Status.Should().Be(ResponseStatus.Success);
            paymentCancellationResult.Result.IsSuccessful.Should().BeTrue();
            paymentCancellationResult.Result.Error.Should().BeNullOrEmpty();
        }
    }
}