using System.Threading.Tasks;
using NasladdinPlace.Core.Models.Feedback;

namespace NasladdinPlace.Core.Services.Feedback.Builder.SenderInfo
{
    public interface ISenderInfoFactory
    {
        Task<Models.Feedback.SenderInfo> CreateAsync(SenderCreationInfo info);
    }
}