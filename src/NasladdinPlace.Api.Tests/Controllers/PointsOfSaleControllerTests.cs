using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NasladdinPlace.Api.Controllers;
using NasladdinPlace.Api.Dtos.PosAntennasOutputPower;
using NasladdinPlace.Api.Services.WebSocket.Managers.MessageHandlers.Contracts;
using NasladdinPlace.Api.Tests.DependencyInjection;
using NasladdinPlace.Core.Enums;
using NasladdinPlace.TestUtils;
using NasladdinPlace.TestUtils.Utils;
using NUnit.Framework;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Tests.Controllers
{
    public class PointsOfSaleControllerTests : TestsBase
    {
        private const int DefaultUserId = 1;
        private const int DefaultPosId = 1;

        private PointsOfSaleController _pointsOfSaleController;

        private Mock<INasladdinWebSocketMessageSender> _mockNasladdinWebSocketMessageSender;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var serviceProvider = TestServiceProviderFactory.Create();

            _mockNasladdinWebSocketMessageSender = serviceProvider.GetRequiredService<Mock<INasladdinWebSocketMessageSender>>();

            _pointsOfSaleController = serviceProvider.GetRequiredService<PointsOfSaleController>();

            _pointsOfSaleController.ControllerContext = ControllerContextFactory.MakeForUserWithId(DefaultUserId);
        }

        [Test]
        public void UpdateAntennasOutputPower_PosAntennasOutputPowerIsValid_ShouldReturnOkResultAndSentPosAntennasPower()
        {
            var isUpdatePosAntennasPowerSent = false;
            _mockNasladdinWebSocketMessageSender
                .Setup(s => s.SetPosAntennasOutputPowerAsync(It.Is<int>(numb => numb == DefaultPosId), It.IsAny<PosAntennasOutputPower>()))
                .Returns(() =>
                {
                    isUpdatePosAntennasPowerSent = true;
                    return Task.CompletedTask;
                });

            var result = _pointsOfSaleController.UpdateAntennasOutputPower(DefaultPosId, new PosAntennasOutputPowerDto
            {
                OutputPower = PosAntennasOutputPower.Dbm20
            });

            result.Should().BeOfType<OkResult>();
            isUpdatePosAntennasPowerSent.Should().BeTrue();
        }
    }
}
