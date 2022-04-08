using System.Threading.Tasks;
using NasladdinPlace.Application.Services.FiscalizationInfos.Models;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Application.Services.FiscalizationInfos.Contracts
{
    public interface IFiscalizationInfosService
    {
        Task<ValueResult<QrCodeStream>> GetQrCodeByFicalizationInfoIdAsync(int fiscalizationInfoId);
    }
}