using System;
using System.Collections.Generic;

namespace NasladdinPlace.Core.Services.CheckOnline.CheckOnlineAgent
{
    public interface ICheckOnlineAgent
    {
        event EventHandler<List<int>> OnFoundPendingFiscalization;

        void Start(TimeSpan timeIntervalBetweenRequests);

        void Stop();

    }
}
