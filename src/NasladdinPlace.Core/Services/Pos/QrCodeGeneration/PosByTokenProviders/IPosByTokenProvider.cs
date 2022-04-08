using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosByTokenProviders
{
    public interface IPosByTokenProvider
    {
        Task<ValueResult<Models.Pos>> TryProvideByTokenAsync(string token);
    }
}