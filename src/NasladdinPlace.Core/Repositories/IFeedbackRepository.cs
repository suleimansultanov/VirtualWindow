using NasladdinPlace.Core.Models.Feedback;

namespace NasladdinPlace.Core.Repositories
{
    public interface IFeedbackRepository
    {
        void Add(Feedback feedback);
    }
}