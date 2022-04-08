using System.Collections.Generic;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Simple.Models;

namespace NasladdinPlace.Core.Services.Check.Simple.Makers.Contracts
{
    public interface ISimpleCheckMaker
    {
        SimpleCheck MakeCheck(PosOperation posOperation);
        IEnumerable<SimpleCheck> MakeChecks(IEnumerable<PosOperation> posOperations);
    }
}
