using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NasladdinPlace.Core.Services.Check.Detailed.Makers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Makers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Mappers.Contracts;
using NasladdinPlace.Core.Services.Check.Simple.Models;

namespace NasladdinPlace.Core.Services.Check.Simple.Makers
{
    public class SimpleCheckMaker : ISimpleCheckMaker
    {
        private readonly IDetailedCheckMaker _detailedCheckMaker;
        private readonly ISimpleCheckMapper _simpleCheckMapper;

        public SimpleCheckMaker(IDetailedCheckMaker detailedCheckMaker, ISimpleCheckMapper simpleCheckMapper)
        {
            _detailedCheckMaker = detailedCheckMaker;
            _simpleCheckMapper = simpleCheckMapper;
        }

        public SimpleCheck MakeCheck(Core.Models.PosOperation posOperation)
        {
            var detailedCheck = _detailedCheckMaker.MakeCheck(posOperation);
            var simpleCheck = _simpleCheckMapper.Transform(detailedCheck);
            return simpleCheck;
        }

        public IEnumerable<SimpleCheck> MakeChecks(IEnumerable<Core.Models.PosOperation> posOperations)
        {
            return posOperations.Select(MakeCheck).ToImmutableList();
        }
    }
}
