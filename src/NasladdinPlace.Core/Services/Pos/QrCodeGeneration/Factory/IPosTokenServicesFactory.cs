using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosByTokenProviders;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.Factory
{
    public interface IPosTokenServicesFactory
    {
        IPosTokenProvider CreatePosTokenProvider();
        IPosByTokenProvider CreatePosByTokenProvider();
    }
}