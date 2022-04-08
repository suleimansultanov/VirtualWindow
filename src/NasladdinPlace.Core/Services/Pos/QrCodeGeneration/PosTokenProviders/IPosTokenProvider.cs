using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders
{
    public interface IPosTokenProvider
    {
        Task<ValueResult<string>> TryProvidePosTokenAsync(int posId);
    }
}