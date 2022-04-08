using System;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Reports.Observable
{
    public class ReportFailuresObservable : IReportFailuresObservable
    {
        public event EventHandler<ErrorBundle> OnFailureOccured;

        public void NotifyReportFailure(ErrorBundle bundle)
        {
            OnFailureOccured?.Invoke(this, bundle);
        }
    }
}