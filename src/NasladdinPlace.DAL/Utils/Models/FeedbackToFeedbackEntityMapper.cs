using NasladdinPlace.Core.Models.Feedback;
using NasladdinPlace.DAL.Entities;

namespace NasladdinPlace.DAL.Utils.Models
{
    public static class FeedbackToFeedbackEntityMapper
    {
        public static FeedbackEntity Map(Feedback feedback)
        {
            return new FeedbackEntity(
                senderInfo: feedback.SenderInfo,
                dateCreated: feedback.DateCreated,
                appVersion: feedback.AppInfo.AppVersion,
                content: feedback.Body.Content
            );
        }

    }
}
