using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Feedback
{
    public class FeedbackService : IFeedbackService
    {
        public event EventHandler<AddedFeedbackEventArgs> OnFeedbackAdded;
        
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public FeedbackService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }
        
        public async Task ProcessFeedbackAsync(Models.Feedback.Feedback feedback)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                unitOfWork.Feedbacks.Add(feedback);
                await unitOfWork.CompleteAsync();
                
                OnFeedbackAdded?.Invoke(this, new AddedFeedbackEventArgs(feedback));
            }
        }
    }
}