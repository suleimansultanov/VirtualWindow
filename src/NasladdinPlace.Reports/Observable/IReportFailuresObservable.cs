using System;
using NasladdinPlace.Core.Services.Shared.Models;

namespace NasladdinPlace.Reports.Observable
{
    public interface IReportFailuresObservable
    {
        event EventHandler<ErrorBundle> OnFailureOccured;
        void NotifyReportFailure(ErrorBundle bundle);
    }
}