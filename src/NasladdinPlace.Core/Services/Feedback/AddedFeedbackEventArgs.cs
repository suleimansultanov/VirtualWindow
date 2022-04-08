using System;

namespace NasladdinPlace.Core.Services.Feedback
{
    public class AddedFeedbackEventArgs : EventArgs
    {
        public Models.Feedback.Feedback AddedFeedback { get; }

        public AddedFeedbackEventArgs(Models.Feedback.Feedback feedback)
        {
            AddedFeedback = feedback;
        }
    }
}