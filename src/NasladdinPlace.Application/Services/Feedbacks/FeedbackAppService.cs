using System;
using System.Threading.Tasks;
using NasladdinPlace.Application.Dtos.Feedback;
using NasladdinPlace.Application.Services.Feedbacks.Contracts;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Feedback;
using NasladdinPlace.Core.Services.Feedback;
using NasladdinPlace.Core.Services.Feedback.Builder.SenderInfo;

namespace NasladdinPlace.Application.Services.Feedbacks
{
    public class FeedbackAppService : IFeedbackAppService
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ISenderInfoFactory _senderInfoFactory;

        public FeedbackAppService(IFeedbackService feedbackService, ISenderInfoFactory senderInfoFactory)
        {
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
            _senderInfoFactory = senderInfoFactory ?? throw new ArgumentNullException(nameof(senderInfoFactory));
        }

        public async Task CreateFeedbackAsync(FeedbackDto feedbackDto, ApplicationUser user = null)
        {
            var appInfo = new AppInfo(feedbackDto.AppInfo.AppVersion);
            var deviceInfo = new DeviceInfo(feedbackDto.SenderInfo.DeviceInfo.DeviceName, feedbackDto.SenderInfo.DeviceInfo.OperatingSystem);
            var senderCreationInfo = user == null
                ? new SenderCreationInfo(feedbackDto.SenderInfo.PhoneNumber, deviceInfo)
                : new SenderCreationInfo(user, deviceInfo);
            var senderInfo = await _senderInfoFactory.CreateAsync(senderCreationInfo);
            var feedbackBody = new FeedbackBody(feedbackDto.Body.Content);

            var feedback = Feedback.NewInstance(senderInfo, feedbackBody, appInfo);

            await _feedbackService.ProcessFeedbackAsync(feedback);
        }
    }
}
