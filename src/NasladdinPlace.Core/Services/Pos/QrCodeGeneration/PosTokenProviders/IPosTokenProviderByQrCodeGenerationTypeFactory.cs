using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders
{
    public interface IPosTokenProviderByQrCodeGenerationTypeFactory
    {
        IPosTokenProvider Create(PosQrCodeGenerationType qrCodeGenerationType);
    }
}