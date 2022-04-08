using System;
using NasladdinPlace.Core.Services.Check.Simple.Models;

namespace NasladdinPlace.Core.Models.Feedback
{
    public class LastPurchaseInfo
    {
        public LastPurchaseInfo(PosOperation posOperation, SimpleCheck lastSimpleCheck)
        {
            if (lastSimpleCheck == null)
                throw new ArgumentNullException(nameof(lastSimpleCheck));
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));

            LastSimpleCheck = lastSimpleCheck;
            PosOperation = posOperation;
        }

        public SimpleCheck LastSimpleCheck { get; }
        public PosOperation PosOperation { get; }
    }
}
