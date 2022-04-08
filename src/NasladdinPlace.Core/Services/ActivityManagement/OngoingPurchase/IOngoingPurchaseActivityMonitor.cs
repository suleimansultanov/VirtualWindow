using System;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.ActivityManagement.OngoingPurchase
{
    public interface IOngoingPurchaseActivityMonitor : IDisposable
    {
        event EventHandler<MessageArgumentEventArgs<int>> OnPosBecomeInactive;
        event EventHandler<MessageArgumentEventArgs<int>> OnUserBecomeInactive;
        
        void Start();
        void Stop();
    }
}