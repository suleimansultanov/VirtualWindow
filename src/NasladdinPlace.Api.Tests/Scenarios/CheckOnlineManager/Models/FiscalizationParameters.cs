using NasladdinPlace.Core.Models;
using System;

namespace NasladdinPlace.Api.Tests.Scenarios.CheckOnlineManager.Models
{
    public class FiscalizationParameters
    {
        public PosOperation PosOperation { get; }
        public PosOperationTransaction PosOperationTransaction { get; }

        public FiscalizationParameters(PosOperation posOperation, PosOperationTransaction posOperationTransaction)
        {
            if (posOperation == null)
                throw new ArgumentNullException(nameof(posOperation));
            if (posOperationTransaction == null)
                throw new ArgumentNullException(nameof(posOperationTransaction));

            PosOperation = posOperation;
            PosOperationTransaction = posOperationTransaction;
        }
    }
}
