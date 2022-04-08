using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders
{
    public interface IPosTokenProviderByPosIdProvider
    {
        Task<IPosTokenProvider> ProvideAsync(int posId);
    }
}