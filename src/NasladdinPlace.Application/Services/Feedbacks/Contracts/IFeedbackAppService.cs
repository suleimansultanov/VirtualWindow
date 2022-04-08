using System.Threading.Tasks;
using NasladdinPlace.Application.Dtos.Feedback;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Application.Services.Feedbacks.Contracts
{
    public interface IFeedbackAppService
    {
        Task CreateFeedbackAsync(FeedbackDto feedbackDto, ApplicationUser user = null);
    }
}
