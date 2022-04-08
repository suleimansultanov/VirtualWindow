using FluentAssertions;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Pos.Groups.Models;
using NasladdinPlace.Core.Services.Pos.ScreenResolution.Models;
using NasladdinPlace.Core.Services.Pos.ScreenResolution.Printer;
using Xunit;

namespace NasladdinPlace.Core.Tests.Services.PointOfSale.ScreenResolution.Printer
{
    public class IncorrectPosScreenResolutionInfoEnglishPrinterTests
    {
        private const string ExpectedMessageHeader ="Incorrect screen resolution on POS: ";
        private const string PosName = "Pos 1";
        private const int PosId = 1;

        private const int Width = 1680;
        private const int Height = 1050;

        private readonly IncorrectPosScreenResolutionInfoEnglishPrinter _incorrectPosScreenResolutionInfoEnglishPrinter;

        public IncorrectPosScreenResolutionInfoEnglishPrinterTests()
        {
            _incorrectPosScreenResolutionInfoEnglishPrinter = new IncorrectPosScreenResolutionInfoEnglishPrinter();
        }

        [Fact]
        public void Print_PosScreenResolutionInfoIsGiven_ShouldReturnCorrectPrintMessage()
        {
            var posInfo = new PosShortInfo(PosId, PosName);
            var updatableScreenResolution = new UpdatableScreenResolution();
            updatableScreenResolution.Update(new Models.ScreenResolution(Width, Height));

            var posScreenResolutionInfo = new PosScreenResolutionInfo(posInfo, updatableScreenResolution);

            var posScreenResolutionInfos = new[] {posScreenResolutionInfo};

            var message = _incorrectPosScreenResolutionInfoEnglishPrinter.Print(posScreenResolutionInfos);
            message.Should().Contain(ExpectedMessageHeader);
            message.Should().Contain(PosName);
        }
    }
}
