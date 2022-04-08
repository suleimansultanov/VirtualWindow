using System;

namespace NasladdinPlace.Core.Models.Feedback
{
    public class Feedback : Entity
    {
        public static Feedback NewInstance(
            SenderInfo senderInfo,
            FeedbackBody feedbackBody,
            AppInfo appInfo)
        {
            return new Feedback(senderInfo, feedbackBody, appInfo, DateTime.UtcNow);
        }

        public static Feedback RestoredInstance(
            SenderInfo senderInfo,
            FeedbackBody feedbackBody,
            AppInfo appInfo,
            DateTime dateCreated)
        {
            return new Feedback(senderInfo, feedbackBody, appInfo, dateCreated);
        }
        
        public SenderInfo SenderInfo { get; }
        public FeedbackBody Body { get; }
        public AppInfo AppInfo { get; }
        public DateTime DateCreated { get; }

        private Feedback(
            SenderInfo senderInfo, 
            FeedbackBody body, 
            AppInfo appInfo, 
            DateTime dateCreated)
        {
            SenderInfo = senderInfo;
            Body = body;
            AppInfo = appInfo;
            DateCreated = dateCreated;
        }
    }
}