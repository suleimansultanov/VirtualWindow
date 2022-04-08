using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Configurations;
using NasladdinPlace.Api.Controllers;
using NasladdinPlace.Api.Dtos.PaymentCard;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Services.BankingCardConfirmation.Models;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Seeding.Data;
using NasladdinPlace.TestUtils.Utils;
using NUnit.Framework;
using System.Linq;

namespace NasladdinPlace.Api.Tests.Scenarios.Registration
{
    public class BankingCardConfirmationScenario : TestsBase
    {
        private const int DefaultUserId = 1;

        private PaymentCardsController _paymentCardsController;

        public override void SetUp()
        {
            base.SetUp();

            Seeder.Seed(new UsersDataSet());

            var services = TestServiceProviderFactory.Create();

            _paymentCardsController = services.GetRequiredService<PaymentCardsController>();

            var controllerContext = ControllerContextFactory.MakeForUserWithId(DefaultUserId);
            _paymentCardsController.ControllerContext = controllerContext;

            Mapper.Reset();

            Mapper.Initialize(config => config.AddProfile(new MappingProfile()));
        }

        [Test]
        public void ConfirmBankingCard_ValidBankingCardCryptogramWithout3DsIsGiven_ShoudReturnConfirmationSuccess()
        {
            var bankingCardConfirmationDto = new PaymentCardConfirmationDto
            {
                CardCryptogramPacket = "014111111111221102IuUDalV2SXohVf9+dGtsH+hI8W/mgbDyKH9KFZnQTGa/8oSCaFEF54aqR3rpwanlfcRyQhM7ZTebXuRw8vgWvdrkgerDLlcJ2UfVylVHVPUI4TrQ0qGhyx9JmeqGfFlS2vwltZT5IBI9Ci7KAlcuCBHgUc4/epKl3+0Km92LqfBRn0H14spyUT8SZUvVGJOB5eJBlyLzyoQII125KFKc/4fzB8lXTO4Wgj5uXDJcvK/jNh+3TIdXhadUbzw/G+pwsOXNex9XG0gLaDwFslw539PNYXjMK+Ubbdi4KCmZOCYw9GIGSdYeeu/+0pmA/FniWNgJz6XA/6A5KSWxjKHvtw==",
                CardHolder = "TEST USER"
            };

            var result = _paymentCardsController.ConfirmPaymentCardAsync(bankingCardConfirmationDto).Result;
            result.Should().BeOfType<OkObjectResult>();

            var confirmationResponse = (result as OkObjectResult).Value as PaymentCardConfirmationResultDto;
            confirmationResponse.Should().NotBeNull();
            confirmationResponse.Error.Should().BeNullOrEmpty();
            confirmationResponse.Form3DsHtml.Should().BeNullOrEmpty();
            confirmationResponse.PaymentStatus.Should().Be(PaymentCardConfirmationStatus.ConfirmationSucceeded);

            Context.PaymentCards.Should().HaveCount(1);
            Context.Users.Where(u => u.ActivePaymentCardId.HasValue).Should().HaveCount(1);
        }

        [Test]
        public void ConfirmBankingCard_InvalidCryptogramIsGiven_ShouldReturnConfirmationError()
        {
            var bankingCardConfirmationDto = new PaymentCardConfirmationDto
            {
                CardCryptogramPacket = "",
                CardHolder = "TEST USER"
            };

            var result = _paymentCardsController.ConfirmPaymentCardAsync(bankingCardConfirmationDto).Result;
            result.Should().BeOfType<OkObjectResult>();

            var confirmationResponse = (result as OkObjectResult).Value as PaymentCardConfirmationResultDto;
            confirmationResponse.Should().NotBeNull();
            confirmationResponse.Error.Should().NotBeNullOrWhiteSpace();
            confirmationResponse.Form3DsHtml.Should().BeNullOrEmpty();
            confirmationResponse.PaymentStatus.Should().Be(PaymentCardConfirmationStatus.ConfirmationFailed);

            Context.PaymentCards.Should().HaveCount(0);
            Context.Users.Where(u => u.ActivePaymentCardId.HasValue).Should().HaveCount(0);
        }

        [Test]
        public void ConfirmBankingCard_InsufficientFundsCardCryptogramIsGiven_ShouldReturnConfirmationError()
        {
            var bankingCardConfirmationDto = new PaymentCardConfirmationDto
            {
                CardCryptogramPacket = "014000055556221102Y4asdFONN/kPBJSWVQb5kpiwDj8mGjlmywxAzGu5xB9Ye/31SRlgNLfpjW7Ox2SV0bweM71xbblzicfyWtTampLPgroYhyWYmvWDasV66Dp4nseT4RQfU+/8XQVqgOopzvnraMRaNZRk1WGQd56P3VNz/NUPTwVGLltadf246IfUuAglvaIxlx2g9M+9AtcCqq2kkrHRmsNRia3FmNGDbec76xNCTzmBVHia2EauIh9t/v4YzBYnQ9O4HusoFRkzeNPNcxYRZK6ZMuyhXKJROBwJrzRb4civi7upRP5n+3XyhkQdcXWGrmbqU9kRLknM0JaWCMHCJ2Qm7r9MXMVp0Q==",
                CardHolder = "TEST USER"
            };

            var result = _paymentCardsController.ConfirmPaymentCardAsync(bankingCardConfirmationDto).Result;
            result.Should().BeOfType<OkObjectResult>();

            var confirmationResponse = (result as OkObjectResult).Value as PaymentCardConfirmationResultDto;
            confirmationResponse.Should().NotBeNull();
            confirmationResponse.Error.Should().NotBeNullOrWhiteSpace();
            confirmationResponse.Form3DsHtml.Should().BeNullOrEmpty();
            confirmationResponse.PaymentStatus.Should().Be(PaymentCardConfirmationStatus.ConfirmationFailed);

            Context.PaymentCards.Should().HaveCount(0);
            Context.Users.Where(u => u.ActivePaymentCardId.HasValue).Should().HaveCount(0);
        }

        [Test]
        public void ConfirmBankingCard_3DsCardCryptogramIsGiven_ShouldReturnRequirementToPass3Ds()
        {
            var bankingCardConfirmationDto = new PaymentCardConfirmationDto
            {
                CardCryptogramPacket = "014242424242221102glM5qc0yCrKfq+XvYCJy/tpgUjtnNII0tMKESYJvTgaelrEdfxl4cQRNt4Jz6+Nu/FeXWmLvioKlB22mcdN6aXYxK9im1a9IhAFFQL7cD97cdAYOaJMeFr70lsJT9fw67CkZR63mx7Z3XioH9SkOT4y7ecLmQ2N4qEoel9AskewbBtHEsDGJ+B7hprViPCXhwVx5jfNMxFekkFxl63dO8bcYg4rwh19YxXo0py/xPesGjQFR6VmPiVR+ssUMSCQUp3urauD4SRRTslSEzu+uvFbuNbemw7/ZWD249qhfWqz1CQR3bXA32V6xilzOFKgpFCLGOE6oeOe5mSCWrZw/QA==",
                CardHolder = "TEST USER"
            };

            var result = _paymentCardsController.ConfirmPaymentCardAsync(bankingCardConfirmationDto).Result;
            result.Should().BeOfType<OkObjectResult>();

            var confirmationResponse = (result as OkObjectResult).Value as PaymentCardConfirmationResultDto;
            confirmationResponse.Should().NotBeNull();
            confirmationResponse.Error.Should().BeNullOrWhiteSpace();
            confirmationResponse.Info3Ds.Should().NotBeNull();
            confirmationResponse.Info3Ds.AcsUrl.Should().NotBeNullOrWhiteSpace();
            confirmationResponse.Info3Ds.TransactionId.Should().BeGreaterThan(0);
            confirmationResponse.Info3Ds.PaReq.Should().NotBeNullOrWhiteSpace();
            confirmationResponse.Form3DsHtml.Should().NotBeNull();
            confirmationResponse.PaymentStatus.Should().Be(PaymentCardConfirmationStatus.Require3DsAuthorization);

            Context.PaymentCards.Should().HaveCount(0);
            Context.Users.Where(u => u.ActivePaymentCardId.HasValue).Should().HaveCount(0);
        }
    }
}