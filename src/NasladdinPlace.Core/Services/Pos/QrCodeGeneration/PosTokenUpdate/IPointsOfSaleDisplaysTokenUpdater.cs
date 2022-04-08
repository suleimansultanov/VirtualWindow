using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenUpdate
{
    public interface IPointsOfSaleDisplaysTokenUpdater
    {
        Task UpdateAsync();
    }
}