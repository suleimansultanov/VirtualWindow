using System;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Simple.Models;

namespace NasladdinPlace.Core.Services.Check.Simple.Makers.User.Models
{
    public class PosOperationWithCheck
    {
        public SimpleCheck Check { get; }
        public PosOperation CheckPosOperation { get; }

        public PosOperationWithCheck(PosOperation posOperation, SimpleCheck simpleCheck)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));
            if (simpleCheck == null)
                throw new ArgumentNullException(nameof(simpleCheck));

            CheckPosOperation = posOperation;
            Check = simpleCheck;
        }
    }
}
