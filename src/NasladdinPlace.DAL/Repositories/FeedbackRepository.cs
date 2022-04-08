using NasladdinPlace.Core.Models.Feedback;
using NasladdinPlace.Core.Repositories;
using NasladdinPlace.DAL.Utils.Models;

namespace NasladdinPlace.DAL.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly ApplicationDbContext _context;

        public FeedbackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(Feedback feedback)
        {
            var feedbackEntity = FeedbackToFeedbackEntityMapper.Map(feedback);
            _context.Feedbacks.Add(feedbackEntity);
        }
    }
}