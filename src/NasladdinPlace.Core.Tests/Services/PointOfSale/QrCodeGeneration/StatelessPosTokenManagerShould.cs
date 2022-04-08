using FluentAssertions;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.Factory;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NasladdinPlace.Core.Tests.Services.PointOfSale.QrCodeGeneration
{
    public class StatelessPosTokenManagerShould
    {
        private static readonly TimeSpan DefaultTokenValidityPeriod = TimeSpan.FromSeconds(4);

        private const int DefaultPosId = 1;

        private readonly IStatelessPosTokenManager _statelessPosTokenManager;

        public StatelessPosTokenManagerShould()
        {
            var posTokenManagerOptions = new PosTokenServicesOptions
            {
                TokenPrefix = "https://online.nasladdin.club",
                TokenValidityPeriod = DefaultTokenValidityPeriod,
                EncryptionKey = "EncryptionKey"
            };
            _statelessPosTokenManager = PosStatelessTokenManagerFactory.Create(posTokenManagerOptions);
        }

        [Fact]
        public void GenerateNonEmptyToken()
        {
            var token = _statelessPosTokenManager.GeneratePosToken(DefaultPosId);
            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GenerateATokenAndEnsureItValidWhenProvidedTheSamePosIdThatUsedOnGeneration()
        {
            var token = _statelessPosTokenManager.GeneratePosToken(DefaultPosId);
            token.Should().NotBeNullOrEmpty();

            var isTokenValid = _statelessPosTokenManager.IsPosTokenValid(DefaultPosId, token);
            isTokenValid.Should().BeTrue();
        }

        [Fact]
        public void GenerateATokenAndEnsureItIsValidAtTheMiddleOfValidityPeriod()
        {
            var token = _statelessPosTokenManager.GeneratePosToken(DefaultPosId);
            token.Should().NotBeNullOrEmpty();

            Task.Delay(DefaultTokenValidityPeriod / 2).Wait();

            var isTokenValid = _statelessPosTokenManager.IsPosTokenValid(DefaultPosId, token);
            isTokenValid.Should().BeTrue();
        }

        [Fact]
        public void GenerateATokenAndEnsureItIsInvalidAfterTheEndOfValidityPeriod()
        {
            var token = _statelessPosTokenManager.GeneratePosToken(DefaultPosId);
            token.Should().NotBeNullOrEmpty();

            Task.Delay(DefaultTokenValidityPeriod + TimeSpan.FromMilliseconds(500)).Wait();

            var isTokenValid = _statelessPosTokenManager.IsPosTokenValid(DefaultPosId, token);
            isTokenValid.Should().BeFalse();
        }

        [Fact]
        public void GenerateATokenAndEnsureItIsInvalidWhenProvidedOtherPosId()
        {
            var token = _statelessPosTokenManager.GeneratePosToken(DefaultPosId);
            token.Should().NotBeNullOrEmpty();

            var isTokenValid = _statelessPosTokenManager.IsPosTokenValid(DefaultPosId + 1, token);
            isTokenValid.Should().BeFalse();
        }

        [Fact]
        public void IndicateInvalidTokenWhenTryToValidateRandomGuid()
        {
            var isTokenValid = _statelessPosTokenManager.IsPosTokenValid(DefaultPosId, Guid.NewGuid().ToString());
            isTokenValid.Should().BeFalse();
        }

        [Fact]
        public void ReturnCorrectPosIdWhenProvidedCorrectToken()
        {
            var token = _statelessPosTokenManager.GeneratePosToken(DefaultPosId);
            token.Should().NotBeNullOrEmpty();

            _statelessPosTokenManager.TryGetPosIdIfTokenValid(token, out var posId).Should().BeTrue();
            posId.Should().Be(DefaultPosId);
        }

        [Fact]
        public void ReturnFailureResultOfPosIdWhenProvidedIncorrectToken()
        {
            var incorrectToken = Guid.NewGuid().ToString();
            _statelessPosTokenManager.TryGetPosIdIfTokenValid(incorrectToken, out _).Should().BeFalse();
        }
    }
}