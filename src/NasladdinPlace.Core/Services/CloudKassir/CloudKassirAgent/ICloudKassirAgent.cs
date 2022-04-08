using System;
using System.Collections.Generic;

namespace NasladdinPlace.Core.Services.CloudKassir.CloudKassirAgent
{
    public interface ICloudKassirAgent
    {
        event EventHandler<List<int>> OnFoundPendingFiscalization;

        void Start(TimeSpan timeIntervalBetweenRequests);

        void Stop();
    }
}
