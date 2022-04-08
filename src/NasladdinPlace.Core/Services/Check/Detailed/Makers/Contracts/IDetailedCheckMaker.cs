using System.Collections.Generic;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Detailed.Models;

namespace NasladdinPlace.Core.Services.Check.Detailed.Makers.Contracts
{
    public interface IDetailedCheckMaker
    {
        DetailedCheck MakeCheck(PosOperation operation);
        ICollection<DetailedCheck> MakeChecks(ICollection<PosOperation> operations);
    }
}