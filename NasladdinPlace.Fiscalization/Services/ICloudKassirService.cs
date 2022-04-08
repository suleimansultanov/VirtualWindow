using NasladdinPlace.CloudPaymentsCore;
using NasladdinPlace.Fiscalization.Models;
using System.Threading.Tasks;

namespace NasladdinPlace.Fiscalization.Services
{
    public interface ICloudKassirService
    {
        Task<Response<FiscalizationResult>> MakeFiscalizationAsync(FiscalizationRequest fiscalizationRequest);
        Task<Response<CheckInfoResult>> GetFiscalCheckAsync(CheckInfoRequest checkInfoRequest);
        Task<Response<string>> GetFiscalCheckStatusAsync(CheckInfoRequest checkInfoRequest);
    }
}
