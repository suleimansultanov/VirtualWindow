using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.CommonModels;
using NasladdinPlace.Core.Services.Check.Detailed.Models;

namespace NasladdinPlace.Core.Services.Check.Detailed.Mappers.Contracts
{
    public interface IDetailedCheckGoodInstanceCreator
    {
        DetailedCheckGoodInstance Create(
            CheckItem item,
            CheckFiscalizationInfo detailedCheckFiscalizationInfo,
            PosOperation posOperation,
            string fiscalizationQrCodeUrlTemplate,
            string fiscalCheckUrlTemplate
        );
    }
}