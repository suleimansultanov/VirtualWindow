using System;

namespace NasladdinPlace.Core.Services.UnpaidPurchases.Monitor
{
    public interface IUnpaidPurchasesMonitor : IDisposable
    {
        void Start();
        void Stop();
    }
}