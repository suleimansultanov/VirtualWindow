using NasladdinPlace.Core.Services.Check.Detailed.Models;
using NasladdinPlace.Core.Services.Check.Simple.Models;

namespace NasladdinPlace.Core.Services.Check.Simple.Mappers.Contracts
{
    public interface ISimpleCheckMapper
    {
        SimpleCheck Transform(DetailedCheck detailedCheck);
    }
}
