using FluentAssertions;
using NasladdinPlace.Core.Services.Pos.Groups.Models;
using NasladdinPlace.Core.Services.Pos.Version.Models;
using NasladdinPlace.Core.Services.Pos.Version.Printer;
using Xunit;

namespace NasladdinPlace.Core.Tests.Services.PointOfSale.Version.Printer
{
    public class PointsOfSaleVersionUpdateInfoEnglishPrinterShould
    {
        private const string MinRequiredPosVersion = "1.0.0.0";

        private const string ExpectedMessageHeader =
            "Version update up to " + MinRequiredPosVersion + " is required for POS: ";

        private const string PosName = "Pos 1";
        private const int PosId = 1;
        private const string PosVersion = "0.0.0.12345";

        private readonly PointsOfSaleVersionUpdateInfoEnglishPrinter _pointsOfSaleVersionUpdateInfoPrinter;

        public PointsOfSaleVersionUpdateInfoEnglishPrinterShould()
        {
            _pointsOfSaleVersionUpdateInfoPrinter = new PointsOfSaleVersionUpdateInfoEnglishPrinter();
        }
        
        [Fact]
        public void PrintMessageWithMinRequiredVersionAndTheCurrentPosVersionWhenOnePosVersionInfoIsGiven()
        {
            var posInfo = new PosShortInfo(PosId, PosName);
            var posVersionInfo = new PosVersionInfo(posInfo, PosVersion);
            var pointsOfSaleUpdateInfo = new PointsOfSaleVersionUpdateInfo(
                MinRequiredPosVersion, new[] {posVersionInfo}
            );
            var message = _pointsOfSaleVersionUpdateInfoPrinter.Print(pointsOfSaleUpdateInfo);
            message.Should().Contain(ExpectedMessageHeader);
            message.Should().Contain(PosName);
        }
    }
}