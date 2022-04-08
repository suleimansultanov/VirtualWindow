using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Feedback
{
    public interface IFeedbackService
    {
        event EventHandler<AddedFeedbackEventArgs> OnFeedbackAdded;
        Task ProcessFeedbackAsync(Models.Feedback.Feedback feedback);
    }
}